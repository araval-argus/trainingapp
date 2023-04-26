import { Component, Input, OnInit } from '@angular/core';
import { recentChat } from 'src/app/core/models/recentChat-model';
import { ChatService } from 'src/app/core/service/chat-service';
import { HubService } from 'src/app/core/service/hub-service';

@Component({
  selector: 'app-chat-sidebar',
  templateUrl: './chat-sidebar.component.html',
  styleUrls: ['./chat-sidebar.component.scss']
})
export class ChatSidebarComponent implements OnInit {
  recentChats: recentChat[] = [];
  constructor(private chatService: ChatService, private hubService: HubService) { }
  selectedUserName: string = '';
  ngOnInit(): void {
    this.reloadRecent();
    this.chatService.reloadInbox.subscribe((data: any) => {
      this.selectedUserName = data.userName;
    })

    this.hubService.hubConnection.on("receiveChat", (data) => {
      let temp = this.recentChats.find(e => e.to.userName === data.to.userName);
      let unreadCount = 0;
      if (temp != null) {
        unreadCount = temp.unreadCount + 1;
      }
      if (this.selectedUserName == data.to.userName) {
        unreadCount = 0;
      }
      this.recentChats = this.recentChats.filter(e => e.to.userName != data.to.userName);
      this.recentChats.push({
        to: {
          firstName: data.to.firstName,
          lastName: data.to.lastName,
          email: data.to.email,
          imagePath: data.to.imagePath,
          userName: data.to.userName,
        },
        chatContent: {
          content: data.chatContent.content,
          sentAt: new Date(data.chatContent.sentAt),
          type: data.chatContent.type
        },
        unreadCount: unreadCount
      })
      this.recentChats.sort((a, b) => b.chatContent.sentAt.getTime() - a.chatContent.sentAt.getTime())
    })
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
            status: element.to.status
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