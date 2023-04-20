import { Component, ElementRef, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GroupModel } from 'src/app/core/models/group-model';
import { GroupChatModel } from 'src/app/core/models/groupChat-model';
import { GroupNewChat } from 'src/app/core/models/groupNewChat-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';
import { GroupService } from 'src/app/core/service/group-service';

@Component({
  selector: 'app-group-message',
  templateUrl: './group-message.component.html',
  styleUrls: ['./group-message.component.scss']
})
export class GroupMessageComponent implements OnInit {
  //logged In User's data
  loggedInUser: LoggedInUser;
  //Thumbnail Image
  thumbnail;

  //To Toggle Emoji Bar
  isEmojiPickerVisible: boolean;

  //To get Information About Group
  selectedGroup: GroupModel = {
    name: '',
    description: ''
  };

  //To manage media File
  url;
  format;
  unsafeUrl;

  //For New Message
  groupNewChat: GroupNewChat = {
    Content: '',
    GroupId: -1,
    Type: 'text'
  };
  //To get Value's of Chat
  @ViewChild('chatBar', { static: false }) chatBar: ElementRef


  //Chats To load
  chats: GroupChatModel[] = [];

  //replying Chat Id
  replyChatId: number = -1;
  replyingContent: string;


  //To manage Uploaded File
  file: File;

  //File Modal
  @ViewChild('fileModal', { static: false }) fileModal: TemplateRef<any>
  constructor(private groupService: GroupService, private authService: AuthService, private chatService: ChatService, private modalService: NgbModal, private domSanitizer: DomSanitizer,) { }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.isEmojiPickerVisible = false;
    this.groupService.groupSelection.subscribe((data: GroupModel) => {
      this.thumbnail =
        this.selectedGroup.id = data.id,
        this.selectedGroup.name = data.name,
        this.selectedGroup.description = data.description,
        this.thumbnail = data.profileImage != null ? this.setProfileImage(data.profileImage) : "https://via.placeholder.com/37x37";
      this.chats = [];
      this.reloadChat();
    })
  }


  //reload Chat 
  reloadChat() {
    this.cancelReply();
    this.groupService.getAllChat(this.selectedGroup.name).subscribe((data: any) => {
      data.chats.forEach(element => {
        this.pushMessage(element);
      });
      this.chats.sort((a, b) => a.time.getTime() - b.time.getTime());
    })
  }

  //Reply feature TBI
  replyTo(id: number) {
    this.replyingContent = this.chats.find(e => e.Id == id).Content;
    this.replyChatId = id;
  }

  cancelReply() {
    this.replyChatId = -1;
    this.replyingContent = null;
  }

  //getFileURL
  getSrc(image: string) {
    return this.chatService.getFile(image);
  }

  //Checking format of the file
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

  //Emoji
  addEmoji(event: any) {
    this.chatBar.nativeElement.value += event.emoji.native;
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
    this.modalService.open(this.fileModal, {}).result.then(() => {
    }).catch(() => {
    });
  }


  sendFile() {
    const formData: FormData = new FormData();
    formData.append('File', this.file);
    formData.append('GroupId', this.selectedGroup.id.toString());
    formData.append('RepliedId', this.replyChatId.toString());
    console.log(formData);
    this.groupService.addFile(formData).subscribe((data: any) => {
      this.pushMessage(data.chat);
    })
  }


  //On Enter Sent the Chat
  sendChat(event) {
    event.preventDefault();
    const chat = this.chatBar.nativeElement.value;
    this.chatBar.nativeElement.value = "";
    if (chat.trim() != '') {
      this.groupNewChat = {
        Content: chat,
        GroupId: this.selectedGroup.id,
        RepliedId: this.replyChatId != -1 ? this.replyChatId : null
      }
      this.groupService.addMessage(this.groupNewChat).subscribe((data: any) => {
        this.pushMessage(data.chatAdded);
      })
    }
    this.cancelReply()
  }

  //Convert File to URL
  setProfileImage(file: string) {
    return this.groupService.fetchImage(file);
  }

  //Add Chat To Array
  pushMessage(msg: any) {
    this.chats.push({
      Id: msg.id,
      Content: msg.content,
      GroupId: msg.groupId,
      Sender: msg.senderUserName,
      time: new Date(msg.sentAt),
      ReplyingContent: msg.replyingContent,
      ReplyingId: msg.replyingMessageId,
      Type: msg.type,
    })
  }
}
