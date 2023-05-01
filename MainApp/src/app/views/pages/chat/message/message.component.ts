import { AfterViewChecked, AfterViewInit, Component, ElementRef, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges, TemplateRef, ViewChild } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subject, Subscription } from 'rxjs';
import { ChatModel } from 'src/app/core/models/chat-model';
import { loadChatModel } from 'src/app/core/models/loadingChat-model';
import { Profile } from 'src/app/core/models/profile-model';
import { AccountService } from 'src/app/core/service/account-service';
import { ChatService } from 'src/app/core/service/chat-service';
import { HubService } from 'src/app/core/service/hub-service';

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.scss']
})
export class MessageComponent implements OnInit, AfterViewInit, AfterViewChecked, OnDestroy {

  selectedUserImagePath: string = "https://via.placeholder.com/37x37";
  selectedUser: Profile = {
    firstName: '',
    lastName: '',
    userName: '',
    imagePath: ''
  };

  //To manage fecthed Chat
  chatsToLoad = new Array<loadChatModel>();


  //To manage replying Message Component
  replytoMessage: loadChatModel;
  replyingToChat: number = -1;


  //resetting replying feature
  private resetReplySubject: Subject<void> = new Subject();

  //Getting value from form
  @ViewChild('chatForm', { static: false }) chatContent: ElementRef;
  chat: ChatModel; //Setting replying Message
  @ViewChild('scrollMe', { static: false }) scrollMe: ElementRef
  isEmojiPickerVisible: boolean;
  //To save file


  //To display Modal
  url;
  format;
  unsafeUrl;
  file: File;
  @ViewChild('basicModal', { static: false }) content: TemplateRef<any>;

  //To manage Subscription
  reloadInboxSub: Subscription;

  constructor(private chatService: ChatService, private accountService: AccountService, private hubService: HubService, private modalService: NgbModal, private domSanitizer: DomSanitizer) {

  }

  ngOnInit(): void {

    this.isEmojiPickerVisible = false
    this.reloadInboxSub = this.chatService.reloadInbox.subscribe((data: any) => {
      console.log(data);
      this.selectedUser.firstName = data.firstName,
        this.selectedUser.lastName = data.lastName,
        this.selectedUser.userName = data.userName,
        this.selectedUser.status = data.status
      if (data.imagePath != null) {
        this.selectedUserImagePath = this.accountService.fetchImage(data.imagePath);
      } else {
        this.selectedUserImagePath = "https://via.placeholder.com/43x43";
      }
      this.reloadChat();
    })


    //In case user cancel the replying to chat
    this.resetReplySubject.subscribe(() => {
      this.replytoMessage = null;
      this.replyingToChat = -1;
    })

    //Sending File
    this.chatService.sendFileSub.subscribe(() => {
      this.sendFile();
    })

    //replying to message
    this.chatService.replyToChat.subscribe((data => {
      this.replyingToChat = data;
      this.replyToChat();
    }))


    this.hubService.hubConnection.on("receiveChat", (data) => {
      if (this.selectedUser.userName === data.to.userName) {
        this.hubService.hubConnection.send("seenMessage", data.to.userName)
        this.chatsToLoad.push({
          sent: false,
          id: data.chatContent.id,
          content: data.chatContent.content,
          sentAt: new Date(data.chatContent.sentAt),
          replyToChat: data.chatContent.replyToChat,
          replyToContent: data.chatContent.replyToChat == -1 ? null : this.chatsToLoad.find(e => e.id == data.chatContent.replyToChat).content,
          isRead: data.chatContent.isRead,
          type: data.chatContent.type
        })
      }
    })


    this.hubService.hubConnection.on("seenMessageNotification", (receiver) => {
      if (this.selectedUser.userName == receiver) {
        let unreadChat = this.chatsToLoad.filter(e => e.sent && e.isRead == 0);
        unreadChat.forEach(e => e.isRead = 1);
      }
    })


    this.hubService.hubConnection.on("userIsOnline", (user) => {
      console.log(user + " is online");
    })


    this.hubService.hubConnection.on("userStatusUpdated", (userName, statusId) => {
      if (this.selectedUser.userName == userName) {
        this.selectedUser.status = statusId
      }
    })
  }

  //To scroll To bottom
  ngAfterViewInit(): void {

  }

  ngAfterViewChecked(): void {
    try {
      this.scrollMe.nativeElement.scrollTop = this.scrollMe.nativeElement.scrollHeight;
    } catch {
    }

  }


  ngOnDestroy(): void {
    this.selectedUser.userName = '';
    this.reloadInboxSub.unsubscribe();
  }

  replyTo(id: number) {
    this.chatService.replyToChat.next(id);
  }


  reloadChat() {
    this.chatsToLoad = [];
    this.chatService.getChat(this.selectedUser.userName).subscribe((data: any) => {
      this.loadChat(data);
    })
    this.resetReply();
  }




  // This Method will convert CHATDTO to chat model to render on screen.
  loadChat(data: any) {
    var sent = data.chats[0];
    var recieved = data.chats[1];
    //We had to intialize replyToContent null cause chatsToLoad will only complete after all chats are initialize.
    if (sent != null && sent.chatList != null) {
      sent.chatList.forEach(element => {
        this.chatsToLoad.push({
          sent: true,
          id: element.id,
          content: element.content,
          sentAt: new Date(element.sentAt),
          replyToChat: element.replyToChat,
          replyToContent: null,
          isRead: element.isRead,
          type: element.type
        })
      });
    }
    if (recieved != null && recieved.chatList != null) {
      recieved.chatList.forEach(element => {
        this.chatsToLoad.push({
          sent: false,
          id: element.id,
          content: element.content,
          sentAt: new Date(element.sentAt),
          replyToChat: element.replyToChat,
          replyToContent: null,
          isRead: element.isRead,
          type: element.type
        })

      });
    }

    //This will add content to all chat if it's reply of other chat && user can't find chats that not belongs to him
    this.chatsToLoad.forEach(e => {
      e.replyToContent = e.replyToChat != -1 ? this.chatsToLoad.find(el => el.id == e.replyToChat).content.substring(0, 50) : null;
    })
    this.chatsToLoad.sort((a, b) => a.sentAt.getTime() - b.sentAt.getTime());

  }


  //Set message as replying to message
  replyToChat() {
    this.replytoMessage = this.chatsToLoad.find(c => c.id == this.replyingToChat);
  }


  //reset replying to message
  resetReply() {
    this.resetReplySubject.next();
  }


  // Purpose of not to set sender here is security, intruder can change the sender username in request
  sendChat(event: Event) {
    event.preventDefault();
    const chat = this.chatContent.nativeElement.value;
    this.chatContent.nativeElement.value = "";
    this.chat = {
      replyToChat: this.replyingToChat,
      to: this.selectedUser.userName,
      content: chat,
    }
    this.replyingToChat = -1;
    this.chatService.addChat(this.chat).subscribe((data: any) => {
      this.chatsToLoad.push({
        sent: true,
        id: data.savedChat.id,
        content: data.savedChat.content,
        sentAt: new Date(data.savedChat.sentAt),
        replyToChat: data.savedChat.replyToChat,
        replyToContent: data.savedChat.replyToChat != -1 ? this.chatsToLoad.find(e => e.id == data.savedChat.replyToChat).content.substring(0, 50) : null,
        isRead: data.savedChat.isRead,
        type: data.savedChat.type
      })
    })
    this.resetReply();
  }


  //TO manage File
  uploadedFile(event) {
    if (event.target.files.length > 0) {
      this.file = event.target.files[0];
    }
    this.convertFile();
    this.displayModal();

  }

  //Converting file to the url for modal
  convertFile() {
    var reader = new FileReader();
    reader.readAsDataURL(this.file);
    if (this.file.type.indexOf('image') > -1) {
      this.format = 'image';
    } else if (this.file.type.indexOf('video') > -1) {
      this.format = 'video';
    } else if (this.file.type.indexOf('audio') > -1) {
      this.format = 'audio';
    }
    reader.onload = (event) => {
      this.unsafeUrl = (<FileReader>event.target).result;
      this.url = this.domSanitizer.bypassSecurityTrustResourceUrl(this.unsafeUrl);
    }
  }

  //displaying modal
  displayModal() {
    this.modalService.open(this.content, {}).result.then(() => {
    }).catch(() => {
    });
  }


  //Will make formdata by using file and selected user's username
  sendFile() {
    const formData: FormData = new FormData();
    formData.append('file', this.file);
    formData.append('to', this.selectedUser.userName);
    formData.append('replyToChat', this.replyingToChat.toString())
    this.chatService.sendFile(formData).subscribe((data: any) => {
      this.chatsToLoad.push({
        sent: true,
        id: data.chats.id,
        content: data.chats.content,
        sentAt: new Date(data.chats.sentAt),
        replyToChat: data.chats.replyToChat,
        replyToContent: data.chats.replyToChat != -1 ? this.chatsToLoad.find(e => e.id == data.chats.replyToChat).content.substring(0, 50) : null,
        isRead: data.chats.isRead,
        type: data.chats.type
      })
    })
  }


  //To return format of the file
  checkFormat(type: string) {
    let format: string = '';
    if (type.indexOf('image') > -1) {
      format = 'image';
    } else if (type.indexOf('video') > -1) {
      format = 'video';
    } else if (type.indexOf('audio') > -1) {
      format = 'audio';
    }
    return format;
  }

  //getFileURL
  getSrc(image: string) {
    return this.chatService.getFile(image);
  }



  //Emoji
  addEmoji(event: any) {
    this.chatContent.nativeElement.value += event.emoji.native;
  }


}
