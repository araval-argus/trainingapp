import { Component, Input, OnInit } from '@angular/core';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { ChatService } from 'src/app/core/service/chat.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-chat-sidebar',
  templateUrl: './chat-sidebar.component.html',
  styleUrls: ['./chat-sidebar.component.scss']
})
export class ChatSidebarComponent implements OnInit {

  @Input() userObj: LoggedInUser;
  @Input() allUsers: LoggedInUser[];
  @Input() allUsersLoadingFlag: boolean;

  recentChatUsers: LoggedInUser[] = [];
  recentChatUserLoadingFlag: boolean = true;

  defaultNavActiveId = 1;

  constructor(private chatService: ChatService) { }

  ngOnInit(): void {
    this.getRecentChatUsers();
  }


  // utilities
  makeProfileUrl(filePath: string) {
    if (filePath == null) {
      return "https://via.placeholder.com/37x37";
    }
    
    return environment.hostUrl + '/' + filePath;
  }

  getRecentChatUsers() {
    this.chatService.getRecentChatUsers().subscribe(
      (res) => {
        this.recentChatUsers = res;
        this.recentChatUserLoadingFlag = false;
        console.log("Recentusers");
        console.log(res);
        
      },
      (err) => {
        console.log(err);
        
      }
    )
  }

}
