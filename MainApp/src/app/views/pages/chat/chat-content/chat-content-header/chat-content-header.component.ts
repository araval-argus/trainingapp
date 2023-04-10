import { Component, Input, OnInit } from '@angular/core';
import { FriendProfile } from 'src/app/core/models/friend-profile-model';
import { ChatService } from 'src/app/core/service/chat-service';

@Component({
  selector: 'app-chat-content-header',
  templateUrl: './chat-content-header.component.html',
  styleUrls: ['./chat-content-header.component.scss']
})
export class ChatContentHeaderComponent implements OnInit {

  @Input() selectedFriend: FriendProfile;

  constructor(private chatService: ChatService) {
    console.log("chatcontentheader constructor")
   }

  ngOnInit(): void {
    console.log("inside chat content header :- ", this.selectedFriend)

  }

  // back to chat-list for tablet and mobile devices
  backToChatList() {
    document.querySelector('.chat-content').classList.toggle('show');
  }

}
