import { AfterViewChecked, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ColleagueModel } from 'src/app/core/models/colleague-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { MessageDisplayModel } from 'src/app/core/models/message-display-model';
import { MessageModel } from 'src/app/core/models/message-model';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';
import { SignalRService } from 'src/app/core/service/signalr-service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-chat-load',
  templateUrl: './chat-load.component.html',
  styleUrls: ['./chat-load.component.scss']
})
export class ChatLoadComponent implements OnInit , AfterViewChecked{

  localPath : string = environment.ImageUrl;
  basicModalCode: any;
  username : string;
  selUser: ColleagueModel;
  selUserImage : string;
  loggedInUser : LoggedInUser;
  imageSource : string;
  msg : MessageModel ;
  displayMsgList : MessageDisplayModel[] = [];
  isReplying : Boolean = false;
  rplyMsg : string ;
  rplyMsgLen : number;
  rplyMsgId: number;
  imageFile : File;
  todayDate : Date = new Date();
  @ViewChild('scrollMe') private myScrollContainer: ElementRef;

  constructor(private route: ActivatedRoute , private chatService : ChatService , private authService:AuthService ,
     private modalService: NgbModal , private signalRService : SignalRService) { }

  ngOnInit(): void {
    this.msg = {
      content: '',
      messageFrom: '',
      messageTo: '',
      repliedTo:-1,
      seen :0,
      type:''
    };

    this.signalRService.hubConnection.on('recieveMessage',(msg:MessageDisplayModel)=>{
      this.displayMsgList.push(msg);
    })

    this.scrollToBottom();

    this.route.params.subscribe(
      (params : Params) =>{
        this.username = params['username'];

        this.chatService.getUser(this.username).subscribe((data:ColleagueModel)=>{
          this.selUser = data;
          this.selUserImage = environment.ImageUrl + data.imagePath;

          this.chatService.fetchMessages(this.selUser.userName).subscribe((data:MessageDisplayModel[])=>{
            this.displayMsgList = data;
           // console.log(this.displayMsgList);
          })
        })
      });

    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.imageSource = environment.ImageUrl + this.loggedInUser.imagePath;

    this.chatService.getUser(this.username).subscribe((data:ColleagueModel)=>{
      this.selUser = data;
      this.selUserImage = environment.ImageUrl + data.imagePath;

      this.chatService.fetchMessages(this.selUser.userName).subscribe((data:MessageDisplayModel[])=>{
        this.displayMsgList = data;
      })
    })
  }

  ngAfterViewChecked():void {
    this.scrollToBottom();
  }


  onMessage(message:HTMLInputElement){
    if(message.value.trim()==''){}
    else{
    this.msg.content = message.value;
    this.msg.messageFrom = this.loggedInUser.userName;
    this.msg.messageTo = this.selUser.userName;
    if(this.isReplying)
    {
      this.msg.repliedTo = this.rplyMsgId;
    }
    else{
      this.msg.repliedTo = -1;
    }
    this.msg.seen=0;
    this.msg.type=null;
    this.signalRService.hubConnection.invoke('sendMsg',this.msg).catch((error)=>console.log(error));
    message.value = null;
    this.closeRplyMsg();

    }
  }

  toggleReplyMsg(popupmessage:MessageDisplayModel){
    this.isReplying = true;
    this.rplyMsg = popupmessage.content;
    this.rplyMsgId = popupmessage.id;
    this.rplyMsgLen = popupmessage.content.length;
  }

  closeRplyMsg(){
    this.isReplying = false;
    this.rplyMsg = '';
    this.rplyMsgId = -1;
    this.rplyMsgLen = 0;
  }

  openBasicModal(content) {
    this.modalService.open(content, {}).result.then((result) => {
    }).catch((res) => {});
  }

  onFileSelected(event){
    if (event.target.files.length > 0) {
      this.imageFile = (event.target as HTMLInputElement).files[0];
    }
  }

  uploadFile(){
    console.log('Sending.....');
    const formdata = new FormData();
    formdata.append('File',this.imageFile);
    formdata.append('MessageFrom',this.loggedInUser.userName),
    formdata.append('MessageTo',this.selUser.userName),
    this.chatService.sendFileMessage(formdata).subscribe((data:MessageDisplayModel)=>{
      this.chatService.DidAMessage.next();
    })
  }

  scrollToBottom(): void {
    try {
        this.myScrollContainer.nativeElement.scrollTop = this.myScrollContainer.nativeElement.scrollHeight;
    } catch(err) { }
  }

}
