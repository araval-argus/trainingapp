import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { FriendProfile } from 'src/app/core/models/friend-profile-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { MessageModel } from 'src/app/core/models/message-model';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';

@Component({
  selector: 'app-chat-content-footer',
  templateUrl: './chat-content-footer.component.html',
  styleUrls: ['./chat-content-footer.component.scss']
})
export class ChatContentFooterComponent implements OnInit {

  @Input() selectedFriend: FriendProfile;
  loggedInUser : LoggedInUser = this.authService.getLoggedInUserInfo();



  @ViewChild('messageInput') messageInput: ElementRef;

  constructor(private chatService: ChatService, private authService : AuthService) { }

  ngOnInit(): void {
  }

  sendMessage(){
    const messageModel: MessageModel = {
      sender: this.loggedInUser.sub,
      reciever: this.selectedFriend.userName,
      message: this.messageInput.nativeElement.value
    };
    this.chatService.sendMessage(messageModel);
  }

}
