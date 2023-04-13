import { Component, Input, OnInit } from '@angular/core';
import { FriendProfileModel } from 'src/app/core/models/friend-profile-model';
import { ChatService } from 'src/app/core/service/chat-service';

@Component({
  selector: 'app-chat-content-header',
  templateUrl: './chat-content-header.component.html',
  styleUrls: ['./chat-content-header.component.scss']
})
export class ChatContentHeaderComponent implements OnInit {

  @Input() selectedFriend: FriendProfileModel;

  constructor(private chatService: ChatService) {   }

  ngOnInit(): void {}

  // back to chat-list for tablet and mobile devices
  backToChatList() {
    document.querySelector('.chat-content').classList.toggle('show');
  }

  replyButtonClicked(event: any){
    console.log(event);
  }

}
