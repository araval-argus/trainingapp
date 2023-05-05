import { Component, Input, OnInit } from '@angular/core';
import { UserModel } from 'src/app/core/models/UserModel';

@Component({
  selector: 'app-chat-content-header',
  templateUrl: './chat-content-header.component.html',
  styleUrls: ['./chat-content-header.component.scss']
})
export class ChatContentHeaderComponent implements OnInit {

  @Input() selectedFriend: UserModel;

  constructor() {   }

  ngOnInit(): void {}

  // back to chat-list for tablet and mobile devices
  backToChatList() {
    document.querySelector('.chat-content').classList.toggle('show');
  }

  replyButtonClicked(event: any){
    console.log(event);
  }

}
