import { Component, Input, Output, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { RecentChatUsers } from 'src/app/core/models/recent-chat-users';
import { ChatService } from 'src/app/core/service/chat.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-chat-sidebar',
  templateUrl: './chat-sidebar.component.html',
  styleUrls: ['./chat-sidebar.component.scss']
})
export class ChatSidebarComponent implements OnInit, OnChanges {

  @Input() userObj: LoggedInUser;
  @Input() allUsers: LoggedInUser[];
  @Input() allUsersLoadingFlag: boolean;

  @Output() selectedUser: EventEmitter<any> = new EventEmitter();
  // @Output() open: EventEmitter<any> = new EventEmitter();

  recentChatUsers: RecentChatUsers[] = [];
  recentChatUserLoadingFlag: boolean = true;

  defaultNavActiveId = 1;

  constructor(private chatService: ChatService, private router: ActivatedRoute, private r: Router) { }

  ngOnInit(): void {
    this.getRecentChatUsers();
  }

  ngOnChanges(changes: SimpleChanges): void {
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
        console.log("Recent users");
        console.log(res);
        
      },
      (err) => {
        console.log(err);
      }
    )
  }  

  clickOnUserToChat(username: string) {
    this.selectedUser.emit();
    // this.r.navigateByUrl('/chat/' + username)
  }

}
