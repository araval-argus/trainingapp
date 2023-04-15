import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { FriendProfileModel } from 'src/app/core/models/friend-profile-model';
import { LoggedInUserModel } from 'src/app/core/models/loggedin-user';
import { MessageModel } from 'src/app/core/models/message-model';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';
import { SignalRService } from 'src/app/core/service/signalR-service';

@Component({
  selector: 'app-chat-content-footer',
  templateUrl: './chat-content-footer.component.html',
  styleUrls: ['./chat-content-footer.component.scss']
})
export class ChatContentFooterComponent implements OnInit {

  @Input() selectedFriend: FriendProfileModel;
  @Input() messageToBeReplied: MessageModel;

  LoggedInUserModel : LoggedInUserModel = this.authService.getLoggedInUserInfo();

  @ViewChild('messageInput') messageInput: ElementRef;

  @ViewChild('fileInput') fileInput: ElementRef;

  @ViewChild('basicModal') basicModal;


  constructor(private chatService: ChatService, private authService : AuthService, private modalService: NgbModal, private signalRService: SignalRService) { }

  ngOnInit(): void {

  }

  sendMessage(){
    const messageModel: MessageModel = {
      senderUserName: this.LoggedInUserModel.sub,
      recieverUserName: this.selectedFriend.userName,
      message: this.messageInput.nativeElement.value,
      repliedToMsg: this.messageToBeReplied? "" + this.messageToBeReplied.id : "-1"
    };

    this.signalRService.sendMessage(messageModel);
    this.messageInput.nativeElement.value = "";
    this.messageToBeReplied = null;
  }

  onFileSelected(){
    if(this.fileInput.nativeElement.files.length>0){
      //open modal
      this.modalService.open(this.basicModal);
    }
  }

  sendFile(){
    let file = this.fileInput.nativeElement.files[0];
    console.log(file)

    const formData = new FormData();
    formData.append("file",file);
    formData.append("senderUserName", this.LoggedInUserModel.sub);
    formData.append("recieverUserName", this.selectedFriend.userName);

    console.log(formData.get("file"));

    //http request to add file
    this.chatService.sendFile(formData).subscribe(data => {
      console.log(data)
    })
  }

}
