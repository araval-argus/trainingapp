import { Component, Input, OnInit } from '@angular/core';
import { recentChat } from 'src/app/core/models/recentChat-model';
import { ChatService } from 'src/app/core/service/chat-service';

@Component({
  selector: 'app-chat-sidebar',
  templateUrl: './chat-sidebar.component.html',
  styleUrls: ['./chat-sidebar.component.scss']
})
export class ChatSidebarComponent implements OnInit {
  recentChats: recentChat[] = [];
  constructor(private chatService: ChatService) { }

  ngOnInit(): void {
    this.reloadRecent();
  }

  reloadRecent() {
    this.recentChats = [];
    this.chatService.recentChat().subscribe((data: any) => {
      data.chats.forEach(element => {
        this.recentChats.push({
          to: {
            firstName: element.to.firstName,
            lastName: element.to.lastName,
            email: element.to.email,
            imagePath: element.to.imagePath,
            userName: element.to.userName,
          },
          chatContent: {
            content: element.chatContent.content,
            sentAt: new Date(element.chatContent.sentAt),
            type: element.chatContent.type
          },
          unreadCount: element.unreadCount
        })
      })
      this.recentChats.sort((a, b) => b.chatContent.sentAt.getTime() - a.chatContent.sentAt.getTime());
    })
  }

}
