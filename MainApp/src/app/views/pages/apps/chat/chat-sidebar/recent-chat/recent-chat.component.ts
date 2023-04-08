import { Component, Input, OnInit } from '@angular/core';
import { recentChat } from 'src/app/core/models/recentChat-model';

@Component({
  selector: 'app-recent-chat',
  templateUrl: './recent-chat.component.html',
  styleUrls: ['./recent-chat.component.scss']
})
export class RecentChatComponent implements OnInit {
  @Input('recentChat') recentChat: recentChat;
  thumbnail: string = "https://via.placeholder.com/37x37";
  constructor() { }

  ngOnInit(): void {
  }

}
