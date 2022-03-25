import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Chat } from '../models/chat';
import { LoggedInUser } from '../models/loggedin-user';
import { RecentChatUsers } from '../models/recent-chat-users';
import { AuthService } from './auth-service';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  constructor(private httpClient:HttpClient, private authService: AuthService) { }

  getDirectChatOfUser(user: string): Observable<Chat[]> {
    return this.httpClient.get<Chat[]>( environment.apiUrl + "/chat/direct/" + user)
  }

  sendTextMessage(userTo: string, content: string, replyTo: number ): Observable<{status: string, message: Chat}> {
    let userFrom: string = this.authService.getLoggedInUserInfo().sub;

    let reqObj = {
      sender : userFrom,
      receiver : userTo,
      type: "text",
      content: content ,
      replyTo: replyTo
    }

    return this.httpClient.post<{status: string, message: Chat}>(environment.apiUrl + "/chat/direct/" + userTo, reqObj)
  }

  getRecentChatUsers():Observable<RecentChatUsers[]> {
    let userName: string = this.authService.getLoggedInUserInfo().sub;
    return this.httpClient.get<RecentChatUsers[]>(environment.apiUrl + "/chat/getRecentChatUsers/" + userName);
  }

  markConversationAsRead(friendName: string): Observable<void> {
    return this.httpClient.get<void>(environment.apiUrl + "/chat/markConversationAsRead/" + friendName)
  }

}
