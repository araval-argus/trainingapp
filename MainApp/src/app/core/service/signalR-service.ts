import { Injectable } from "@angular/core";
import * as signalR from "@microsoft/signalr";
import { Subject } from "rxjs";
import { environment } from "src/environments/environment";
import { GroupMemberModel } from "../models/group-member-model";
import { GroupMessageModel } from "../models/group-message-model";
import { MessageModel } from "../models/message-model";
import { NotificationModel } from '../models/Notification-model';
import { AuthService } from "./auth-service";

@Injectable({
  providedIn: "root",
})
export class SignalRService {
  connection: signalR.HubConnection;

  messageAdded: Subject<MessageModel> = new Subject<MessageModel>();
  groupMessageAdded: Subject<GroupMessageModel> =
    new Subject<GroupMessageModel>();
  memberRemoved: Subject<GroupMemberModel> = new Subject<GroupMemberModel>();
  newGroupAdded: Subject<Number> = new Subject<Number>();
  newMemberAdded: Subject<GroupMemberModel> = new Subject<GroupMemberModel>();
  groupRemoved: Subject<string> = new Subject<string>();
  profileUpdated: Subject<void> = new Subject<void>();
  profileDeleted: Subject<void> = new Subject<void>();
  notificationRaised: Subject<NotificationModel> = new Subject<NotificationModel>();
  addNotification: Subject<NotificationModel> = new Subject<NotificationModel>();
  notificationRemoved: Subject<string> = new Subject<string>();

  constructor(private authService: AuthService) {
    this.registerAllSignalREvents();
  }

  makeConnection() {
    if (this.connection.state !== "Connected") {
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

  markMsgAsSeen(messageModel: MessageModel) {
    this.connection.send("MarkMessageAsSeen", messageModel);
  }

  logout() {
    if (this.connection.state === "Connected") {
      this.connection.send(
        "LogoutUser",
        this.authService.getLoggedInUserInfo().sub
      );
    }
  }

  stopConnection() {
    this.connection
      .stop()
      .then(() => {
        console.log("connection stopped");
      })
      .catch((err) => {
        console.log(err);
      });
  }

  sendGroupMessage(groupMessageModel: GroupMessageModel) {
    this.connection.send("AddGroupMessage", groupMessageModel);
  }

  registerAllSignalREvents() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.apiUrl + "/../hubs/chats", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets,
      })
      .build();
    this.connection.on("AddMessageToTheList", (message: MessageModel) => {
      this.messageAdded.next(message);
    });
    this.connection.on("StopConnection", () => {
      this.stopConnection();
    });
    this.connection.on(
      "AddGroupMessageToTheList",
      (groupMessage: GroupMessageModel) => {
        this.groupMessageAdded.next(groupMessage);
      }
    );
    this.connection.on("AddNotification", (data: NotificationModel) => {
      this.notificationRaised.next(data);
    });
    this.connection.on("NewGroupAdded", (groupId: Number) => {
      this.newGroupAdded.next(groupId);
    });
    this.connection.on("RemoveGroup", (memberUserName: string) => {
      this.groupRemoved.next(memberUserName);
    });
    this.connection.on("AddGroupMember", (newMember: GroupMemberModel) => {
      this.newMemberAdded.next(newMember);
    });
    this.connection.on("MemberRemoved", (member: GroupMemberModel) => {
      this.memberRemoved.next(member);
    });
    this.connection.on("ProfileUpdated", () => {
      this.profileUpdated.next();
    });
    this.connection.on("ProfileDeleted", () => {
      this.profileDeleted.next();
    });

  }
}
