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

  messageAdded: EventEmitter<MessageModel> = new EventEmitter<MessageModel>();

  constructor(private authService: AuthService) {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.apiUrl + "/../hubs/chats", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .build();
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
    this.connection.on("AddMessageToTheList", (message: MessageModel) => {

      this.messageAdded.emit(message);
    });
  }

  registerUser() {
    this.connection.send(
      "RegisterUser",
      this.authService.getLoggedInUserInfo().sub
    );
  }

  sendMessage(messageModel: MessageModel) {
    this.connection.send("AddMessage", messageModel);
  }

  logout() {
    if (this.connection.state === "Connected") {
      this.connection
        .send("LogoutUser", this.authService.getLoggedInUserInfo().sub)
        .then(() => {
          console.log("user logged out");
        });
    }
  }
}
