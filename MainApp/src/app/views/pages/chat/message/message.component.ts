import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { loadChatModel } from 'src/app/core/models/loadingChat-model';
import { ChatService } from 'src/app/core/service/chat-service';

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.scss']
})
export class MessageComponent implements OnInit {

  @Input() message: loadChatModel
  me: boolean = false;
  url;
  unsafeUrl;
  format;
  replyingToChatContent;

  constructor(private chatService: ChatService) { }

  ngOnInit(): void {
    this.me = this.message.sent;
    if (this.message.type.indexOf('image') > -1) {
      this.format = 'image';
    } else if (this.message.type.indexOf('video') > -1) {
      this.format = 'video';
    } else if (this.message.type.indexOf('audio') > -1) {
      this.format = 'audio';
    }
    this.url = this.chatService.getFile(this.message.content);

  }

  replyTo(id: number) {
    this.chatService.replyToChat.next(id);
  }

  // goToMessage(event: Event) {
  //   event.preventDefault();
  //   this.chatService.scrollToChat.next(this.message.replyToChat);
  // }
}
