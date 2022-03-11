import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Chat } from '../models/chat';
import { LoggedInUser } from '../models/loggedin-user';
import { AuthService } from './auth-service';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  constructor(private httpClient:HttpClient, private authService: AuthService) { }

  getDirectChatOfUser(user: string): Observable<Chat[]> {
    return this.httpClient.get<Chat[]>( environment.apiUrl + "/chat/direct/" + user)
  }

  sendTextMessage(userTo: string, content: string): Observable<{status: string, message: Chat}> {
    let userFrom: string = this.authService.getLoggedInUserInfo().sub;

    let reqObj = {
      sender : userFrom,
      receiver : userTo,
      type: "text",
      content: content 
    }

    return this.httpClient.post<{status: string, message: Chat}>(environment.apiUrl + "/chat/direct/" + userTo, reqObj)
  }

  getRecentChatUsers():Observable<LoggedInUser[]> {
    let userName: string = this.authService.getLoggedInUserInfo().sub;
    return this.httpClient.get<LoggedInUser[]>(environment.apiUrl + "/chat/getRecentChatUsers/" + userName);
  }

}
