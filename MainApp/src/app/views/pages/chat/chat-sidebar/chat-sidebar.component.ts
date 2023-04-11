import { Component, Input, OnInit } from '@angular/core';
import { FriendProfile } from 'src/app/core/models/friend-profile-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';

@Component({
  selector: 'app-chat-sidebar',
  templateUrl: './chat-sidebar.component.html',
  styleUrls: ['./chat-sidebar.component.scss']
})
export class ChatSidebarComponent implements OnInit {

  defaultNavActiveId = 1;
  loggedInUser: LoggedInUser;

  @Input() friends: FriendProfile[];

  constructor(private authService: AuthService,
    private chatService: ChatService) { }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    //console.log("inside chat-sidebar oninit:- ", this.loggedInUser)
  }

  onClickUser(friend){
    this.chatService.friendSelected.emit(friend);
    this.chatService.fetchMessages(this.authService.getLoggedInUserInfo().sub, friend.userName).subscribe((data)=>{
      this.chatService.messagesRecieved.emit(data);
    });
  }
}
