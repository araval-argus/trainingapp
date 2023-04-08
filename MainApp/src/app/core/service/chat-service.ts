import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: "root",
})
export class ChatService {

  constructor(private http : HttpClient){}

  fetchFriendsName(searchTerm: string) {
    return this.http.get(environment.apiUrl + "/chat/fetchFriends",{
      params: new HttpParams().append("searchTerm", searchTerm)
    });
  }
}
