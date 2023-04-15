import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
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

  localPath : string = environment.ImageUrl;
  basicModalCode: any;
  username : string;
  selUser: ColleagueModel;
  selUserImage : string;
  loggedInUser : LoggedInUser;
  ImageSource : string;
  msg : MessageModel ;
  displayMsgList : MessageDisplayModel[] = [];
  IsReplying : Boolean = false;
  RplyMsg : string ;
  RplyMsgId: number;
  ImageFile : File;
  //To Scroll to Bottom
  @ViewChild(ScrollToBottomDirective)
  scroll: ScrollToBottomDirective;

  constructor(private route: ActivatedRoute , private chatService : ChatService , private authService:AuthService , private modalService: NgbModal) { }

  ngOnInit(): void {
    this.msg = {
      Content: '',
      MessageFrom: '',
      MessageTo: '',
      RepliedTo:-1,
      Seen :0,
      Type:''
    };

    this.route.params.subscribe(
      (params : Params) =>{
        this.username = params['username'];

        this.chatService.GetUser(this.username).subscribe((data:ColleagueModel)=>{
          this.selUser = data;
          this.selUserImage = environment.ImageUrl + data.imagePath;

          this.chatService.FetchMessages(this.selUser.userName).subscribe((data:MessageDisplayModel[])=>{
            this.displayMsgList = data;
           // console.log(this.displayMsgList);
          })
        })
      });

    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.ImageSource = environment.ImageUrl + this.loggedInUser.ImagePath;

    this.chatService.GetUser(this.username).subscribe((data:ColleagueModel)=>{
      this.selUser = data;
      this.selUserImage = environment.ImageUrl + data.imagePath;

      this.chatService.FetchMessages(this.selUser.userName).subscribe((data:MessageDisplayModel[])=>{
        console.log(data);
        this.displayMsgList = data;
        console.log(this.displayMsgList);
      })
    })
  }

  onMessage(message:HTMLInputElement){
    if(message.value.trim()==''){}
    else{
    this.msg.Content = message.value;
    this.msg.MessageFrom = this.loggedInUser.UserName;
    this.msg.MessageTo = this.selUser.userName;
    if(this.IsReplying)
    {
      this.msg.RepliedTo = this.RplyMsgId;
    }
    else{
      this.msg.RepliedTo = -1;
    }
    this.msg.Seen=0;
    this.msg.Type=null;
    this.chatService.doMessage(this.msg).subscribe((data:MessageDisplayModel)=>{
      this.displayMsgList.push(data);
      this.chatService.DidAMessage.next();
    });
    message.value = null;
    this.CloseRplyMsg();
    }
  }

  ToggleReplyMsg(popupmessage:MessageDisplayModel){
    this.IsReplying = true;
    this.RplyMsg = popupmessage.content;
    this.RplyMsgId = popupmessage.id;
  }

  CloseRplyMsg(){
    this.IsReplying = false;
    this.RplyMsg = '';
  }

  openBasicModal(content) {
    this.modalService.open(content, {}).result.then((result) => {
    }).catch((res) => {});
  }

  onFileSelected(event){
    if (event.target.files.length > 0) {
      this.ImageFile = (event.target as HTMLInputElement).files[0];
    }
  }

  UploadFile(){
    console.log('Sending.....');
    const formdata = new FormData();
    formdata.append('File',this.ImageFile);
    formdata.append('MessageFrom',this.loggedInUser.UserName),
    formdata.append('MessageTo',this.selUser.userName),
    this.chatService.sendFileMessage(formdata).subscribe((data:MessageDisplayModel)=>{
      console.log(data);
      this.displayMsgList.push(data);
      console.log(this.displayMsgList);
      this.chatService.DidAMessage.next();
    })
  }

}
