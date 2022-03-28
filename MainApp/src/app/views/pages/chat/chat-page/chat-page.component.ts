import { Component, ElementRef, Input, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { Chat } from 'src/app/core/models/chat';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { ChatService } from 'src/app/core/service/chat.service';
import { UserService } from 'src/app/core/service/user.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-chat-page',
  templateUrl: './chat-page.component.html',
  styleUrls: ['./chat-page.component.scss']
})
export class ChatPageComponent implements OnInit {

  @Input() userObj: LoggedInUser;
  @Input() userToChat: string;

  @ViewChildren('messages') messages: QueryList<any>;
  @ViewChild('content') content: ElementRef;

  userToChatObj: LoggedInUser;

  chatList: Chat[] = [];

  chatMessage: string;

  replyToMsgIndicator: Chat;

  //flag
  userToChatLoadingFlag: boolean = false;
  chatLoadingFlag: boolean = true;

  constructor(private userService: UserService, private chatService: ChatService) { }

  ngOnInit(): void {

    this.chatMessage = "";

    this.userService.getUserByUserName(this.userToChat).subscribe(
      (res) => {
        this.userToChatLoadingFlag = true;
        this.userToChatObj = res;
        console.log("This is the counter person");
        console.log(res);

        this.getDirectChatWithUser(this.userToChatObj.userName);
      },
      (err) => {
        console.log(err);
        
      }
    )

    this.chatService.markConversationAsRead(this.userToChat).subscribe();
  }

  ngAfterViewInit() {
    this.scrollToBottom();
    this.messages.changes.subscribe(this.scrollToBottom);
  }
  
  scrollToBottom = () => {
    try {
      this.content.nativeElement.scrollTop = this.content.nativeElement.scrollHeight;
    } catch (err) {}
  }

  getDirectChatWithUser(userToChat: string) {
    this.chatService.getDirectChatOfUser(userToChat).subscribe(
      (res) => {
        this.chatList = res;
        this.chatLoadingFlag = false;
        console.log(res);
        
      },
      (err) => {
        console.log(err);
        alert('Error in loading chat!')
      }
    )
  }

  sendMessage() {
    let msgReplyId: number = 0;

    if (this.replyToMsgIndicator) {
      msgReplyId = this.replyToMsgIndicator.id;
    }

    this.chatService.sendTextMessage(this.userToChatObj.userName, this.chatMessage, msgReplyId).subscribe(
      (res) => {
        if (res.status.toLowerCase() == "success") {
          this.chatList.push(res.message);
          this.chatMessage = "";
          this.replyToMsgIndicator = null;
        }
      },
      (err) => {
        console.log(err);
        
      }
    )
  }

  setReplyToMsg(chatMsg: Chat) {
    this.replyToMsgIndicator = chatMsg;
  }

  unsetReplyToMsg() {
    this.replyToMsgIndicator = null;
  }

  // utilities
  makeProfileUrl(filePath: string) {
    if (filePath == null) {
      return "https://via.placeholder.com/37x37";
    }
  
    return environment.hostUrl + '/' + filePath;
  }

}
