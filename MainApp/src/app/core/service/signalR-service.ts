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
    if (this.connection.state !== "Connected") {
      this.connection.start().then(
        () => {
          console.log("connected successfully");
          this.registerUser();
          this.connection.on("AddMessageToTheList", (message: MessageModel) => {
            this.messageAdded.emit(message);
          });
          this.connection.on("StopConnection", () => {
            this.stopConnection()
          });
        },
        (err) => {
          console.log(err.message);
        }
      );

    }
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
    console.log("logout method");
    if (this.connection.state === "Connected") {
      console.log("is in connected");
      console.log(this.authService.getLoggedInUserInfo());
      this.connection.send(
        "LogoutUser",
        this.authService.getLoggedInUserInfo().sub
      );
    }
  }

  stopConnection(){
    this.connection.stop().then( () => {
      console.log("connection stopped")
    }).catch( (err) => {
      console.log(err)
    })
  }
}
