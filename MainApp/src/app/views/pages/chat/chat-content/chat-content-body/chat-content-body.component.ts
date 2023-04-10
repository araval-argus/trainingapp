import { Component, Input, OnInit } from '@angular/core';
import { FriendProfile } from 'src/app/core/models/friend-profile-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { MessageModel } from 'src/app/core/models/message-model';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';

@Component({
  selector: 'app-chat-content-body',
  templateUrl: './chat-content-body.component.html',
  styleUrls: ['./chat-content-body.component.scss']
})
export class ChatContentBodyComponent implements OnInit {

  @Input() selectedFriend: FriendProfile
  loggedInUser: LoggedInUser = this.authService.getLoggedInUserInfo();

  messages: MessageModel[] = [];

  constructor(private chatService: ChatService, private authService: AuthService) { }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    console.log("inside chat body selected friend:- ", this.selectedFriend)
    console.log("inside chat body loggedInuser:- ", this.loggedInUser);


    this.chatService.messagesRecieved.subscribe(data => {
      this.messages = data.messages;
    });
  }

}
