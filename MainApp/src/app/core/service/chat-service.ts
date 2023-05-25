import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { UserModel } from "../models/UserModel";
import { AuthService } from "./auth-service";
import { MessageModel } from "../models/message-model";
import { Subject } from 'rxjs';

@Injectable({
  providedIn: "root",
})
export class ChatService {

  constructor(private http : HttpClient, private authService : AuthService){}

  friendSelected = new Subject<UserModel>();
  messagesRecieved = new Subject<MessageModel[]>();
  messageSent = new Subject<MessageModel>();

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

  fetchAllInteractedUsers(loggedInUsername: string){
    return this.http.get(environment.apiUrl+ "/chat/fetchAllInteractedUsers",{
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

  sendFile(formData: FormData){
    return this.http.post(environment.apiUrl + "/chat/addFile", formData);
  }

  fetchUser(userName: string){
    return this.http.get(environment.apiUrl + "/account/FetchUser", {
      params: new HttpParams().append("userName", userName)
    });
  }
}
