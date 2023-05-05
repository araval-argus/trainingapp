import { Component, Input, OnInit } from '@angular/core';
import { UserModel } from 'src/app/core/models/UserModel';
import { LoggedInUserModel } from 'src/app/core/models/loggedin-user';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';

@Component({
  selector: 'app-chat-sidebar',
  templateUrl: './chat-sidebar.component.html',
  styleUrls: ['./chat-sidebar.component.scss']
})
export class ChatSidebarComponent implements OnInit {

  defaultNavActiveId = 1;
  loggedInUser: LoggedInUserModel;
  today = new Date().getDate();

  @Input() friends: UserModel[];

  constructor(private authService: AuthService,
    private chatService: ChatService) { }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
  }

  onClickUser(friend){
    friend.unreadMessageCount = 0;
    this.chatService.friendSelected.next(friend);
    this.chatService.fetchMessages(this.authService.getLoggedInUserInfo().sub, friend.userName).subscribe((data)=>{
      this.chatService.messagesRecieved.next(data);
    });
  }

  markAsRead(event: Event, friend: UserModel){
    event.preventDefault();
    event.stopPropagation();
    this.chatService.markMsgAsRead(this.loggedInUser.sub, friend.userName).subscribe(msg=>{
      friend.unreadMessageCount = 0;
    })
  }
}
