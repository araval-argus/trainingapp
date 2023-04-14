import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { environment } from "src/environments/environment";
import { MessageModel } from "../models/message-model";
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

  GetUser(username : string){
    return this.http.get(environment.apiUrl+ "/account/" + username);
  }

  doMessage(msg : MessageModel){
    return this.http.post(environment.apiUrl + "/Chat/Message" , msg);
  }

  FetchMessages(seluserusername:string){
    return this.http.get(environment.apiUrl + "/Chat/MsgList" + seluserusername);
  }

  LoadRecentChat(){
    return this.http.get(environment.apiUrl + "/Chat/RecentChat")
  }

}
