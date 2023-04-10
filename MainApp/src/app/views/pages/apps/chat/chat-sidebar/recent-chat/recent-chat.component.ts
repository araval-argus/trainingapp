import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { recentChat } from 'src/app/core/models/recentChat-model';
import { AccountService } from 'src/app/core/service/account-service';
import { ChatService } from 'src/app/core/service/chat-service';

@Component({
  selector: 'app-recent-chat',
  templateUrl: './recent-chat.component.html',
  styleUrls: ['./recent-chat.component.scss']
})
export class RecentChatComponent implements OnInit {
  @Input('recentChat') recentChat: recentChat;
  @Output() selectedUser = new EventEmitter<any>();
  thumbnail: string = "https://via.placeholder.com/37x37";
  constructor(private accountService: AccountService, private chatService: ChatService) { }

  ngOnInit(): void {
    if (this.recentChat.to.imagePath != null) {
      this.thumbnail = this.accountService.fetchImage(this.recentChat.to.imagePath);
    }
  }

  sendUser(item: any) {

    this.chatService.reloadInbox.next(item);
  }

}
