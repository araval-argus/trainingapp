import { AfterViewChecked, AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FriendProfile } from 'src/app/core/models/friend-profile-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { MessageModel } from 'src/app/core/models/message-model';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';

@Component({
  selector: 'app-chat-content-body',
  templateUrl: './chat-content-body.component.html',
  styleUrls: ['./chat-content-body.component.scss']
})
export class ChatContentBodyComponent implements OnInit, AfterViewChecked {

  @Input() selectedFriend: FriendProfile
  @Output() replyButtonClicked = new EventEmitter<MessageModel>();

  @ViewChild("scrollbar") scrollbar: ElementRef;

  loggedInUser: LoggedInUser = this.authService.getLoggedInUserInfo();

  messages: MessageModel[] = [];

  constructor(private chatService: ChatService, private authService: AuthService) { }



  ngAfterViewChecked(){
     try{
      const element = this.scrollbar.nativeElement;
      element.scrollTop = element.scrollHeight - element.clientHeight;
    }catch(e){}
  }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.chatService.messagesRecieved.subscribe(data => {
      this.messages = data.messages;
      console.log("messages",this.messages);
      console.log("logged in user", this.loggedInUser);
    });
  }

  replyToThisMessage(message){
    //console.log("reply button clicked")
    this.replyButtonClicked.emit(message);
  }
}
