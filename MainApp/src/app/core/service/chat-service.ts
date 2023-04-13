import { HttpClient, HttpParams } from "@angular/common/http";
import { EventEmitter, Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { FriendProfileModel } from "../models/friend-profile-model";
import { AuthService } from "./auth-service";
import { MessageModel } from "../models/message-model";

@Injectable({
  providedIn: "root",
})
export class ChatService {

  constructor(private http : HttpClient, private authService : AuthService){}

  friendSelected = new EventEmitter<FriendProfileModel>();
  messagesRecieved = new EventEmitter<MessageModel[]>();

  fetchFriendsName(searchTerm: string) {
    return this.http.get(environment.apiUrl + "/chat/fetchFriends",{
      params: new HttpParams().append("searchTerm", searchTerm)
    });
  }

  fetchMessages(loggedInUserName: string, friendUserName: string){
    return this.http.get<MessageModel[]>(environment.apiUrl + "/chat/fetchMessages",{
      params: new HttpParams().append("loggedInUserName", loggedInUserName).append("friendUserName",friendUserName)
    });
  }

  fetchAll(loggedInUsername: string){
    return this.http.get(environment.apiUrl+ "/chat/fetchAll",{
      params: new HttpParams().append("loggedInUsername", loggedInUsername)
    });
  }

  sendMessage(messageModel: MessageModel){
    this.http.post(environment.apiUrl + "/chat/addMessage", messageModel).subscribe();
  }

  markMsgAsRead(loggedInUserName: string, friendUserName: string){
    return this.http.get(environment.apiUrl + "/chat/markAsRead", {
      params: new HttpParams().append("loggedInUserName", loggedInUserName).append("friendUserName", friendUserName)
    })
  }
}
