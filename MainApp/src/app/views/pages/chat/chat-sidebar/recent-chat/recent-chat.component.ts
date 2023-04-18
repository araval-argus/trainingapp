import { Component, Input, OnInit } from '@angular/core';
import { recentChat } from 'src/app/core/models/recentChat-model';
import { AccountService } from 'src/app/core/service/account-service';
import { ChatService } from 'src/app/core/service/chat-service';
import { HubService } from 'src/app/core/service/hub-service';

@Component({
  selector: 'app-recent-chat',
  templateUrl: './recent-chat.component.html',
  styleUrls: ['./recent-chat.component.scss']
})
export class RecentChatComponent implements OnInit {
  @Input('recentChat') recentChat: recentChat;
  contentToDisplay: string = '';
  thumbnail: string = "https://via.placeholder.com/37x37";
  constructor(private accountService: AccountService, private chatService: ChatService, private hubService: HubService) { }

  ngOnInit(): void {
    if (this.recentChat.to.imagePath != null) {
      this.thumbnail = this.accountService.fetchImage(this.recentChat.to.imagePath);
    }
    if (this.recentChat.chatContent.content.length > 50) {
      this.contentToDisplay = this.recentChat.chatContent.content.substring(0, 20) + "...";
    } else if (this.recentChat.chatContent.type != "text") {
      this.contentToDisplay = this.recentChat.chatContent.type;
    } else {
      this.contentToDisplay = this.recentChat.chatContent.content;
    }

  }

  openChat(item: any) {
    this.recentChat.unreadCount = 0;
    this.hubService.hubConnection.send("seenMessage", item.userName);
    this.chatService.reloadInbox.next(item);
  }


  markAsRead(event: Event) {
    event.preventDefault();
    event.stopPropagation();
    this.chatService.markAsRead(this.recentChat.to.userName).subscribe(data => {
      console.log(data);
    })
  }
}
