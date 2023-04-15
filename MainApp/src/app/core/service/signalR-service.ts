import { EventEmitter, Injectable } from "@angular/core";
import * as signalR from "@microsoft/signalr";
import { environment } from "src/environments/environment";
import { MessageModel } from '../models/message-model';
import { AuthService } from './auth-service';

@Injectable({
  providedIn: "root",
})
export class SignalRService {
  connection: signalR.HubConnection;

  messageAdded : EventEmitter<MessageModel> = new EventEmitter<MessageModel>();

  constructor(private authService: AuthService) {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.apiUrl + "/../hubs/chats", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .build();

      this.connection.on("AddMessageToTheList", (message: MessageModel) => {
        console.log(message)
        this.messageAdded.emit(message);
      })
  }

  makeConnection() {
      this.connection.start().then(
        () => {
          console.log("connected successfully");
          this.registerUser();
        },
        (err) => {
          console.log(err.message);
        }
      );
  }

  registerUser() {
      this.connection.send(
        "RegisterUser",
        this.authService.getLoggedInUserInfo().sub
      );

  }

  sendMessage(messageModel: MessageModel){
    this.connection.send(
      "AddMessage",
      messageModel
    )
  }

}
