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
  repliedChatId: number = -1;
  constructor(private chatService: ChatService) { }

  ngOnInit(): void {
    if (this.message.sent) {
      this.me = true;
    }
    if (this.message.replyToChat != -1) {
      this.repliedChatId = this.message.replyToChat;
    }
    console.log(this.message);
  }

  replyTo(id: number) {
    this.chatService.replyToChat.next(id);
  }

  // goToMessage(event: Event) {
  //   event.preventDefault();
  //   this.chatService.scrollToChat.next(this.repliedChatId);
  // }
}
