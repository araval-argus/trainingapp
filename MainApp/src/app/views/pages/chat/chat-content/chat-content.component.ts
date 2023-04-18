import { Component,  Input, OnInit } from '@angular/core';
import { FriendProfileModel } from 'src/app/core/models/friend-profile-model';
import { MessageModel } from 'src/app/core/models/message-model';


@Component({
  selector: 'app-chat-content',
  templateUrl: './chat-content.component.html',
  styleUrls: ['./chat-content.component.scss']
})
export class ChatContentComponent implements OnInit {

  @Input() selectedFriend: FriendProfileModel;
  @Input() messageToBeReplied: MessageModel;

  constructor() { }

  ngOnInit(): void {

  }

  replyButtonClicked(message: MessageModel){
    this.messageToBeReplied = message;
  }

}
