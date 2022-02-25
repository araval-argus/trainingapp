import { Component, Input, OnInit } from '@angular/core';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';

@Component({
  selector: 'app-chat-sidebar',
  templateUrl: './chat-sidebar.component.html',
  styleUrls: ['./chat-sidebar.component.scss']
})
export class ChatSidebarComponent implements OnInit {

  @Input() userObj: LoggedInUser;

  defaultNavActiveId = 1;

  constructor() { }

  ngOnInit(): void {
  }

}
