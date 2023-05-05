import { Component,  Input, OnInit } from '@angular/core';
import { UserModel } from 'src/app/core/models/UserModel';
import { MessageModel } from 'src/app/core/models/message-model';


@Component({
  selector: 'app-chat-content',
  templateUrl: './chat-content.component.html',
  styleUrls: ['./chat-content.component.scss']
})
export class ChatContentComponent implements OnInit {

  @Input() selectedFriend: UserModel;
  @Input() messageToBeReplied: MessageModel;

  constructor() { }

  ngOnInit(): void {

  }

  replyButtonClicked(message: MessageModel){
    this.messageToBeReplied = message;
  }

}
