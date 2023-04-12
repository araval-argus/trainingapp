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
    if (this.message.type == 'text') {
      if (this.message.content.length > 50) {
        this.contentToDisplay = this.message.content.substring(0, 50) + "...";
      } else {
        this.contentToDisplay = this.message.content;
      }
    } else {
      if (this.message.type.indexOf('image') > -1) {
        this.contentToDisplay = 'Replying to Image File';
      } else if (this.message.type.indexOf('video') > -1) {
        this.contentToDisplay = 'Replying to Video File';
      } else if (this.message.type.indexOf('audio') > -1) {
        this.contentToDisplay = 'Replying to Audio File';
      }
    }
  }

  cancelReply() {
    this.cancelReplying.emit();
  }
}
