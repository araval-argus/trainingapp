import {
  AfterViewChecked,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnInit,
  Output,
  ViewChild,
} from "@angular/core";
import { UserModel } from "src/app/core/models/UserModel";
import { LoggedInUserModel } from "src/app/core/models/loggedin-user";
import { MessageModel } from "src/app/core/models/message-model";
import { AuthService } from "src/app/core/service/auth-service";
import { ChatService } from "src/app/core/service/chat-service";
import { SignalRService } from "src/app/core/service/signalR-service";
import { environment } from "src/environments/environment";

@Component({
  selector: "app-chat-content-body",
  templateUrl: "./chat-content-body.component.html",
  styleUrls: ["./chat-content-body.component.scss"],
})
export class ChatContentBodyComponent implements OnInit, AfterViewChecked {
  @Input() selectedFriend: UserModel;
  @Output() replyButtonClicked = new EventEmitter<MessageModel>();

  messageToBeReplied: MessageModel;
  messages: MessageModel[] = [];
  loggedInUser: LoggedInUserModel = this.authService.getLoggedInUserInfo();
  today = new Date().getDate();

  @ViewChild("scrollbar") scrollbar: ElementRef;

  constructor(
    private chatService: ChatService,
    private authService: AuthService,
    private signalRService: SignalRService
  ) {}

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.chatService.messagesRecieved.subscribe((data: any) => {
      //for setting the url of file
      data.messages.forEach((element) => {
        element.message = this.setPath(element.messageType) + element.message;
        element.createdAt = new Date(element.createdAt + 'Z')
      });
      this.messages = data.messages;
    });

    this.replyButtonClicked.subscribe((message) => {
      this.messageToBeReplied = message;
    });

    this.chatService.friendSelected.subscribe(() => {
      this.scrollToTheBottom();
    });

    this.signalRService.messageAdded.subscribe((message: MessageModel) => {
      if(message.senderUserName === this.selectedFriend.userName || message.recieverUserName === this.selectedFriend.userName){
        message.message = this.setPath(message.messageType) + message.message;
        message.createdAt = new Date(message.createdAt);
        this.messages.push(message);
        this.scrollToTheBottom();
        this.messageToBeReplied = null;
      }
    });
  }

  ngAfterViewChecked() {
    this.scrollToTheBottom();
  }

  replyToThisMessage(message) {
    console.log(message);
    this.replyButtonClicked.emit(message);
  }

  scrollToTheBottom() {
    const element = this.scrollbar.nativeElement;
    element.scrollTop = element.scrollHeight - element.clientHeight;
  }

  setPath(type: number) {
    switch (type) {
      case 2: {
        return environment.apiUrl + "/../SharedFiles/Audios/";
      }
      case 3: {
        return environment.apiUrl + "/../SharedFiles/Images/";
      }
      case 4: {
        return environment.apiUrl + "/../SharedFiles/Videos/";
      }
      default: {
        return "";
      }
    }
  }
}
