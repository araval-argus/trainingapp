import { Component, OnInit, AfterViewInit, OnDestroy, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, switchMap } from 'rxjs/operators';
import { ChatModel } from 'src/app/core/models/chat-model';
import { loadChatModel } from 'src/app/core/models/loadingChat-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { Profile } from 'src/app/core/models/profile-model';
import { recentChat } from 'src/app/core/models/recentChat-model';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, AfterViewInit, OnDestroy, AfterViewChecked {

  loggedInUser: LoggedInUser
  selectedUser: Profile = {
    firstName: '',
    lastName: '',
    userName: '',
  }

  removeResult: boolean = false;
  hideRightBox: boolean = true;

  imageURL: string;
  selectedUserImagePath: string = null;
  searchQuery: string;

  profiles: Profile[]
  recentChatProfile: recentChat[] = [];
  newRecentChatProfile: recentChat = {
    to: null,
    chatContent: {
      content: null,
      sentAt: null
    },
  }
  defaultNavActiveId = 1;
  chatsToLoad = new Array<loadChatModel>();
  //To store message to be replied
  replytoMessage: loadChatModel;

  chat: ChatModel;
  replyingToChat: number = -1;

  //Manages subscription to be unsubscribe on destroy
  private searchSubscription?: Subscription;
  private reloadNewInbox?: Subscription;
  private replyChatSub?: Subscription;

  //resetting replying feature
  private resetReplySubject: Subject<void>;


  @ViewChild('chatForm', { static: false }) chatContent: ElementRef;
  @ViewChild('searchBar', { static: false }) searchBar: ElementRef;
  @ViewChild('scrollMe') scrollMe: ElementRef

  // To convert user input to the observable
  private readonly searchProfile = new Subject<string | undefined>();


  constructor(private authService: AuthService, private accountService: AccountService, private chatService: ChatService) {
    this.resetReplySubject = new Subject();
  }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.imageURL = localStorage.getItem('imagePath');
    if (this.imageURL == null) {
      this.imageURL = "https://via.placeholder.com/37x37";
    }
    this.searchSubscription = this.searchProfile.pipe(
      debounceTime(1000),//operator waits for 1000 milliseconds
      switchMap((searchQuery) => this.accountService.searchProfiles(searchQuery)),//operator will cancel any ongoing search request if a new search query is emitted, and only the latest search request will be sent to the server.
    ).subscribe(data => {
      this.profiles = data;
    });


    // This will update User every time new user selected
    this.reloadNewInbox = this.chatService.reloadInbox.subscribe((event: any) => {
      this.selectedUser.firstName = event.firstName;
      this.selectedUser.lastName = event.lastName;
      this.selectedUser.userName = event.userName;
      this.selectedUser.imagePath = event.imagePath;
      if (event.imagePath != null) {
        this.selectedUserImagePath = this.accountService.fetchImage(this.selectedUser.imagePath);
      } else {
        this.selectedUserImagePath = "https://via.placeholder.com/43x43";
      }
      this.removeResult = true;
      this.hideRightBox = false;
      this.reloadChat();
    })

    //Replying to chat
    this.replyChatSub = this.chatService.replyToChat.subscribe((data => {
      this.replyingToChat = data;
      this.replyToChat();
    }))

    this.resetReplySubject.subscribe(() => {
      this.replytoMessage = null;
      this.replyingToChat = -1;
    })
  }

  //To navigate to bottom of the list
  ngAfterViewChecked(): void {
    try {
      this.scrollMe.nativeElement.scrollTop =
        this.scrollMe.nativeElement.scrollHeight;
    } catch (err) { }
  }


  ngAfterViewInit(): void {
    // Show chat-content when clicking on chat-item for tablet and mobile devices
    document.querySelectorAll('.chat-list .chat-item').forEach(item => {
      item.addEventListener('click', () => {
        document.querySelector('.chat-content').classList.toggle('show');
      })
    });

    //Chat service observer from message component to scroll to the message
    // this.chatService.scrollToChat.subscribe(data => {
    //   this.scrollToMessage(data);
    // })

  }


  ngOnDestroy(): void {
    this.searchSubscription.unsubscribe();
    this.reloadNewInbox.unsubscribe();
    this.resetReplySubject.unsubscribe();
    this.replyChatSub.unsubscribe();
  }

  // back to chat-list for tablet and mobile devices
  backToChatList() {
    document.querySelector('.chat-content').classList.toggle('show');
  }

  save() {
    console.log('passs');
  }

  //This will search the user based on search string
  fetchProfile(event: Event) {
    this.searchQuery = (event.target as HTMLInputElement).value;
    this.removeResult = false;
    if (this.searchQuery.length >= 3) {
      this.searchProfile.next(this.searchQuery?.trim());
    } else {
      this.profiles = [];
    }
  }

  //This will load inbox based on selected user from search Result
  loadInbox(event) {
    this.chatService.reloadInbox.next(event);
    (this.searchBar.nativeElement as HTMLInputElement).value = "";
  }

  // Purpose of not to set sender here is security, intruder can change the sender username in request
  sendChat() {
    const chat = this.chatContent.nativeElement.value;
    this.chatContent.nativeElement.value = "";
    this.chat = {
      replyToChat: this.replyingToChat,
      to: this.selectedUser.userName,
      content: chat,
    }
    this.chatService.addChat(this.chat).subscribe(() => {
      this.reloadChat();
    })
    this.replyingToChat = -1;
  }


  //this will fetch the all the chats that goes on between user and selected user
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
          replyToContent: null
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
          replyToContent: null
        })
      });
    }
    //This will add content to all chat if it's reply of other chat
    this.chatsToLoad.forEach(e => {
      e.replyToContent = e.replyToChat != -1 ? this.chatsToLoad.find(el => el.id == e.replyToChat).content.substring(0, 30) : null
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

  // scrollToMessage(idOfMessage: number) {
  //   var id = idOfMessage + "";
  //   const element = document.getElementById(id);
  //   element.scrollIntoView({ block: "start", inline: "nearest" });
  // }
}