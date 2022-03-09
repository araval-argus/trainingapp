import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Chat } from '../models/chat';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  constructor(private httpClient:HttpClient) { }

  getDirectChatOfUser(user: string): Observable<Chat[]> {
    return this.httpClient.get<Chat[]>( environment.apiUrl + "/chat/direct/" + user)
  }
}
