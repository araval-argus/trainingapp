import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { UserModel } from 'src/app/core/models/UserModel';
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

  @Input() selectedFriend: UserModel;
  @Input() messageToBeReplied: MessageModel;

  LoggedInUserModel : LoggedInUserModel = this.authService.getLoggedInUserInfo();

  @ViewChild('messageInput') messageInput: ElementRef;
  @ViewChild('fileInput') fileInput: ElementRef;
  @ViewChild('basicModal') basicModal;


  constructor(private chatService: ChatService,
    private authService : AuthService,
    private modalService: NgbModal,
    private signalRService: SignalRService) { }

  ngOnInit(): void {

  }

  sendMessage(){
    const messageModel: MessageModel = {
      senderUserName: this.LoggedInUserModel.sub,
      recieverUserName: this.selectedFriend.userName,
      message: this.messageInput.nativeElement.value,
      repliedToMsg: this.messageToBeReplied? "" + this.messageToBeReplied.id : "-1",
      createdAt: new Date(),
      messageType: 1
    };
    if(messageModel.message !== ""){
      this.signalRService.sendMessage(messageModel);
    }
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

    const formData = new FormData();
    formData.append("file",file);
    formData.append("senderUserName", this.LoggedInUserModel.sub);
    formData.append("recieverUserName", this.selectedFriend.userName);

    this.chatService.sendFile(formData).subscribe(data => {
      console.log(data)
    })
  }

  addEmoji(event: any){
    this.messageInput.nativeElement.value += event.emoji.native;
  }

  openEmojiMart(){
    const emojiMart: HTMLElement = document.querySelector("emoji-mart");
    emojiMart.classList.toggle('d-none');
  }
  cancelReply() {
    this.messageToBeReplied = null;
  }
}