import { Component, Input, OnInit } from '@angular/core';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
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

  defaultNavActiveId = 1;

  constructor() { }

  ngOnInit(): void {
  }


  // utilities
  makeProfileUrl(filePath: string) {
    if (filePath == null) {
      return "https://via.placeholder.com/37x37";
    }
    
    return environment.hostUrl + '/' + filePath;
  }

}
