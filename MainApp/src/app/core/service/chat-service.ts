import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, Subject } from "rxjs";
import { environment } from "src/environments/environment";
import { MessageDisplayModel } from "../models/message-display-model";

@Injectable({
  providedIn: 'root'
})

export class ChatService{

  //To Reflect Change in Recent Chat on Message
  DidAMessage = new Subject<MessageDisplayModel>();

  constructor(private http: HttpClient) { }

  search(searchvalue : string){
    return this.http.get(environment.apiUrl + "/Chat/" + searchvalue );
  }

  getUser(userName : string){
    return this.http.get(environment.apiUrl+ "/account/" + userName);
  }

  fetchMessages(selUserUserName:string){
    return this.http.get(environment.apiUrl + "/Chat/MsgList" + selUserUserName);
  }

  loadRecentChat(){
    return this.http.get(environment.apiUrl + "/Chat/RecentChat");
  }

  markAsRead(selUserUserName:string){
    return this.http.post(environment.apiUrl + "/Chat/MarkAsRead" + selUserUserName , null);
  }

  sendFileMessage( formdata : FormData ){
    return this.http.post(environment.apiUrl + "/Chat/SendFileMessage" , formdata);
}

}
