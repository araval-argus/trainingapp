import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { loadChatModel } from 'src/app/core/models/loadingChat-model';

@Component({
  selector: 'app-reply-message',
  templateUrl: './reply-message.component.html',
  styleUrls: ['./reply-message.component.scss']
})
export class ReplyMessageComponent implements OnInit {


  @Input('message') message: loadChatModel;
  @Output() cancelReplying = new EventEmitter();
  contentToDisplay: string = '';
  constructor() { }

  ngOnInit(): void {
    if (this.message.content.length > 20) {
      this.contentToDisplay = this.message.content.substring(0, 20) + "...";
    } else {
      this.contentToDisplay = this.message.content;
    }
  }

  cancelReply() {
    this.cancelReplying.emit();
  }
}
