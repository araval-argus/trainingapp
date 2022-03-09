import { Component, Input, OnInit } from '@angular/core';
import { Chat } from 'src/app/core/models/chat';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { ChatService } from 'src/app/core/service/chat.service';
import { UserService } from 'src/app/core/service/user.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-chat-page',
  templateUrl: './chat-page.component.html',
  styleUrls: ['./chat-page.component.scss']
})
export class ChatPageComponent implements OnInit {

  @Input() userObj: LoggedInUser;
  @Input() userToChat: string;

  userToChatObj: LoggedInUser;

  chatList: Chat[] = [];

  //flag
  userToChatLoadingFlag: boolean = false;
  chatLoadingFlag: boolean = true;

  constructor(private userService: UserService, private chatService: ChatService) { }

  ngOnInit(): void {
    this.userService.getUserByUserName(this.userToChat).subscribe(
      (res) => {
        this.userToChatLoadingFlag = true;
        this.userToChatObj = res;
        console.log("This is the counter person");
        console.log(res);

        this.getDirectChatWithUser(this.userToChatObj.userName);
      },
      (err) => {
        console.log(err);
        
      }
    )
  }

  getDirectChatWithUser(userToChat: string) {
    this.chatService.getDirectChatOfUser(userToChat).subscribe(
      (res) => {
        this.chatList = res;
        this.chatLoadingFlag = false;
        console.log(res);
        
      },
      (err) => {
        console.log(err);
        alert('Error in loading chat!')
      }
    )
  }


  // utilities
  makeProfileUrl(filePath: string) {
    if (filePath == null) {
      return "https://via.placeholder.com/37x37";
    }
  
    return environment.hostUrl + '/' + filePath;
  }

}
