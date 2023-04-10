import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ChatModel } from "../models/chat-model";
import { environment } from "src/environments/environment";
import { Subject } from "rxjs";

@Injectable({
    providedIn: "root"
})
export class ChatService {
    public readonly reloadInbox = new Subject<Event>();
    public readonly replyToChat = new Subject<number>();
    constructor(private http: HttpClient) { }
    addChat(chat: ChatModel) {
        return this.http.post(environment.apiUrl + "/chat/addChat", chat);
    }
    getChat(username: string) {
        return this.http.get(environment.apiUrl + "/chat/getChat?s=" + username);
    }

    recentChat() {
        return this.http.get(environment.apiUrl + "/chat/recent");
    }
}