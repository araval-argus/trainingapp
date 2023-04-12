import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { ScrollToBottomDirective } from 'src/app/core/helper/scroll-to-bottom.directive';
import { ColleagueModel } from 'src/app/core/models/colleague-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { MessageDisplayModel } from 'src/app/core/models/message-display-model';
import { MessageModel } from 'src/app/core/models/message-model';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-chat-load',
  templateUrl: './chat-load.component.html',
  styleUrls: ['./chat-load.component.scss']
})
export class ChatLoadComponent implements OnInit {

  username : string;
  selUser: ColleagueModel;
  selUserImage : string;
  loggedInUser : LoggedInUser;
  ImageSource : string;
  msg : MessageModel ;
  displayMsgList : MessageDisplayModel[] = [];
  //To Scroll to Bottom
  @ViewChild(ScrollToBottomDirective)
  scroll: ScrollToBottomDirective;

  constructor(private route: ActivatedRoute , private chatService : ChatService , private authService:AuthService) { }

  ngOnInit(): void {
    this.msg = {
      Content: '',
      MessageFrom: '',
      MessageTo: '',
    };

    this.route.params.subscribe(
      (params : Params) =>{
        this.username = params['username'];
      });

    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.ImageSource = environment.ImageUrl + this.loggedInUser.ImagePath;

    this.chatService.GetUser(this.username).subscribe((data:ColleagueModel)=>{
      this.selUser = data;
      this.selUserImage = environment.ImageUrl + data.imagePath;

      this.chatService.FetchMessages(this.selUser.userName).subscribe((data:MessageDisplayModel[])=>{
        this.displayMsgList = data;
      })
    })
  }

  onMessage(message:string){
    this.msg.Content = message;
    this.msg.MessageFrom = this.loggedInUser.UserName;
    this.msg.MessageTo = this.selUser.userName;
    this.chatService.doMessage(this.msg).subscribe((data:MessageDisplayModel)=>{
      this.displayMsgList.push(data);
    });
  }

}
