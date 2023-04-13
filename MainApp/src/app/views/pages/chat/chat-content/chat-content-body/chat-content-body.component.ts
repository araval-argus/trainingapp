import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FriendProfileModel } from 'src/app/core/models/friend-profile-model';
import { LoggedInUserModel } from 'src/app/core/models/loggedin-user';
import { MessageModel } from 'src/app/core/models/message-model';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';

@Component({
  selector: 'app-chat-content-body',
  templateUrl: './chat-content-body.component.html',
  styleUrls: ['./chat-content-body.component.scss']
})
export class ChatContentBodyComponent implements OnInit {

  @Input() selectedFriend: FriendProfileModel
  @Output() replyButtonClicked = new EventEmitter<MessageModel>();

  messageToBeReplied: MessageModel;

  @ViewChild("scrollbar") scrollbar: ElementRef;

  loggedInUser: LoggedInUserModel = this.authService.getLoggedInUserInfo();

  messages: MessageModel[] = [];

  constructor(private chatService: ChatService, private authService: AuthService) { }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.chatService.messagesRecieved.subscribe(data => {
      this.messages = data.messages;
      //console.log(this.messages)
    });
    this.replyButtonClicked.subscribe(message => {
      this.messageToBeReplied = message
    })
    setTimeout( () => {
      try{
      const element = this.scrollbar.nativeElement;
      element.scrollTop = element.scrollHeight - element.clientHeight;
    }catch(e){}
    },300)

  }

  replyToThisMessage(message){
    this.replyButtonClicked.emit(message);
  }

  cancelReply(){
    console.log(this.messageToBeReplied.id);
    this.messageToBeReplied = null;
  }
}
