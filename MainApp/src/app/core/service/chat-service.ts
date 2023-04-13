import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { environment } from "src/environments/environment";
import { ColleagueModel } from "../models/colleague-model";
import { MessageModel } from "../models/message-model";

@Injectable({
  providedIn: 'root'
})


export class ChatService{

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
