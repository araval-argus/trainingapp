import { Component, Input, OnInit } from "@angular/core";
import { LoggedInUser } from "src/app/core/models/user/loggedin-user";
import { RecentChatModel } from "src/app/core/models/chat/recent-chat";
import { ChatService } from "src/app/core/service/chat-service";
import { UserService } from "src/app/core/service/user-service";

@Component({
  selector: "app-chat-sidebar",
  templateUrl: "chat-sidebar.component.html",
})
export class ChatSideBarComponent implements OnInit {
  defaultNavActiveId = 1;
  userMatched = [];
  openMenu = false;
  timeOutId;

  recentChats: RecentChatModel[];
  @Input() user: LoggedInUser;

  constructor(
    private userService: UserService,
    private chatService: ChatService
  ) {}

  ngOnInit() {
    //get recent chat
    this.chatService.getRecentUsers().subscribe((res: RecentChatModel[]) => {
      // console.log(res);
      this.recentChats = res;
    });
  }

  //hide menu
  hideMenu() {
    this.openMenu = false;
  }

  //debouncing of request
  searchUsers(event) {
    if (this.timeOutId) {
      clearTimeout(this.timeOutId);
    }

    this.timeOutId = setTimeout(() => {
      this.userService
        .getUsers(event.target.value)
        .subscribe((res: { data: [] }) => {
          this.userMatched = res.data;
        });
    }, 1000);
  }

  //get users on input
  onInput(event) {
    //if no string is entered
    
    if (event.target.value === null || event.target.value.length === 0) {
      this.userMatched = [];
      clearTimeout(this.timeOutId);
      return;
    }

    this.openMenu = true;
    this.searchUsers(event);
  }

  getProfile(user: LoggedInUser) {
    return this.userService.getProfileUrl(user);
  }

  reloadRecentChat(){
    this.chatService.getRecentUsers().subscribe((res: RecentChatModel[]) => {
      this.recentChats = res;
    });
  }

  getLastMsg(msg : string){
    if(!msg) return 'file';

    if(msg.length > 15){
      return msg.substring(0, 15) + "...";
    }
    return msg;
  }

  //last msg time
  getTime(str : Date){

    let cur = new Date(str);
    
    const yesterday = new Date();

    if(cur.getDate() === yesterday.getDate() &&
    cur.getMonth() === yesterday.getMonth() &&
    cur.getFullYear() === yesterday.getFullYear()){

      var hours = cur.getHours();
      var minutes = '' + cur.getMinutes();
      var ampm = hours >= 12 ? 'PM' : 'AM';
      hours = hours % 12;
      hours = hours ? hours : 12; // the hour '0' should be '12'
      minutes = cur.getMinutes() < 10 ? '0' + minutes : minutes;

      return hours + ':' + minutes + ' ' + ampm;
      
    }

    yesterday.setDate(yesterday.getDate() - 1)

    if(cur.getDate() === yesterday.getDate() &&
      cur.getMonth() === yesterday.getMonth() &&
      cur.getFullYear() === yesterday.getFullYear()){
        return "Yesterday"
    }

    return cur.toLocaleDateString();
  }
}