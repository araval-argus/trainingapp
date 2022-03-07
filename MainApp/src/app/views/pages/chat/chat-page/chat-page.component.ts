import { Component, Input, OnInit } from '@angular/core';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';

@Component({
  selector: 'app-chat-page',
  templateUrl: './chat-page.component.html',
  styleUrls: ['./chat-page.component.scss']
})
export class ChatPageComponent implements OnInit {

  @Input() userObj: LoggedInUser;
  @Input() userToChat: string;

  constructor() { }

  ngOnInit(): void {
  }

}
