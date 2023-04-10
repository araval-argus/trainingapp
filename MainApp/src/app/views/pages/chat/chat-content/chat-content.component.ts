import { Component, Input, OnInit } from '@angular/core';
import { FriendProfile } from 'src/app/core/models/friend-profile-model';

@Component({
  selector: 'app-chat-body',
  templateUrl: './chat-content.component.html',
  styleUrls: ['./chat-content.component.scss']
})
export class ChatContentComponent implements OnInit {

  @Input() selectedFriend: FriendProfile


  constructor() { }

  ngOnInit(): void {

  }

}
