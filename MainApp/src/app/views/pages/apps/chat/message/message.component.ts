import { Component, Input, OnInit } from '@angular/core';
import { loadChatModel } from 'src/app/core/models/loadingChat-model';

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.scss']
})
export class MessageComponent implements OnInit {

  @Input() message: loadChatModel
  me: boolean = false;
  constructor() { }

  ngOnInit(): void {
    if (this.message.sent) {
      this.me = true;
    }
  }

}
