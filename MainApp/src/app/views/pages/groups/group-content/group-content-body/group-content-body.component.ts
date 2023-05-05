import { AfterViewChecked, Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GroupMessageModel } from 'src/app/core/models/group-message-model';
import { GroupModel } from 'src/app/core/models/group-model';
import { LoggedInUserModel } from 'src/app/core/models/loggedin-user';
import { AuthService } from 'src/app/core/service/auth-service';
import { GroupService } from 'src/app/core/service/group.service';
import { SignalRService } from 'src/app/core/service/signalR-service';
import { environment } from 'src/environments/environment';
import { GroupMemberModel } from 'src/app/core/models/group-member-model';

@Component({
  selector: 'app-group-content-body',
  templateUrl: './group-content-body.component.html',
  styleUrls: ['./group-content-body.component.scss']
})
export class GroupContentBodyComponent implements OnInit, AfterViewChecked {

  @Input() selectedGroup: GroupModel;
  @Input() joinedUsers: GroupMemberModel[];

  messages: GroupMessageModel[] = [];
  messageToBeReplied: GroupMessageModel;
  loggedInUser: LoggedInUserModel = this.authService.getLoggedInUserInfo();
  today = new Date().getDate();

  @ViewChild("messageInput") messageInput: ElementRef;
  @ViewChild("scrollbar") scrollbar: ElementRef;
  @ViewChild('fileInput') fileInput: ElementRef;
  @ViewChild('basicModal') basicModal;

  constructor(
    private groupService: GroupService,
    private authService: AuthService,
    private signalRService: SignalRService,
    private modalService: NgbModal
    ) { }

  ngOnInit(): void {

    this.signalRService.groupMessageAdded.subscribe((groupMessage: GroupMessageModel) => {
      groupMessage.senderImage = this.joinedUsers.find( user => user.userName === groupMessage.senderUserName).imageUrl;
      groupMessage.message = this.setPath(groupMessage.messageType) + groupMessage.message;
      groupMessage.createdAt = new Date(groupMessage.createdAt)
      if(groupMessage.groupId === this.selectedGroup.id){
        this.messages.push(groupMessage);
      }
    })

    this.groupService.groupSelected.subscribe( group => {
      this.selectedGroup = group;
      this.fetchMessages();
    })

    this.fetchMessages();
  }

  ngAfterViewChecked(){
    this.scrollToTheBottom();
  }

  sendMessage(){
    if(this.messageInput.nativeElement.value){
      const groupMessageModel: GroupMessageModel = {
        senderUserName: this.loggedInUser.sub,
        groupId: this.selectedGroup.id,
        message: this.messageInput.nativeElement.value,
        repliedToMsg: this.messageToBeReplied? "" + this.messageToBeReplied.id : "-1",
        createdAt: new Date()
      };
      if(groupMessageModel.message.trim() !== ""){
        this.signalRService.sendGroupMessage(groupMessageModel);
      }
      this.messageInput.nativeElement.value = "";
      this.messageToBeReplied = null;
    }
  }

  onFileSelected(){
    if(this.fileInput.nativeElement.files.length>0){
      this.modalService.open(this.basicModal);
    }
  }

  addEmoji(event: any){
    this.messageInput.nativeElement.value += event.emoji.native;
  }

  sendFile(){
     let file = this.fileInput.nativeElement.files[0];

     const formData = new FormData();
     formData.append("file",file);
     formData.append("senderUserName", this.loggedInUser.sub);
     formData.append("groupId", this.selectedGroup.id+"");

    this.groupService.sendFile(formData).subscribe(data => {})
  }

  cancelReply(){
    this.messageToBeReplied = null;
  }

  openEmojiMart(){
    const emojiMart: HTMLElement = document.querySelector("emoji-mart");
    emojiMart.classList.toggle('d-none');
  }

  scrollToTheBottom() {
    const element = this.scrollbar.nativeElement;
    element.scrollTop = element.scrollHeight;
  }

  fetchMessages(){
    this.groupService.fetchMessages(this.loggedInUser.sub, this.selectedGroup.id).subscribe( (messages:GroupMessageModel[]) => {
      messages.forEach( message => {
        message.senderImage = this.joinedUsers.find( user => user.userName === message.senderUserName).imageUrl;
        message.message = this.setPath(message.messageType) + message.message;
        message.createdAt = new Date(message.createdAt + 'Z')
      })
      this.messages = messages;
    })
  }

  replyTothisMessage(message: GroupMessageModel){
    this.messageToBeReplied = message;
  }

  setPath(type: number) {
    switch (type) {
      case 2: {
        return environment.apiUrl + "/../SharedFiles/Audios/";
      }
      case 3: {
        return environment.apiUrl + "/../SharedFiles/Images/";
      }
      case 4: {
        return environment.apiUrl + "/../SharedFiles/Videos/";
      }
      default: {
        return "";
      }
    }
  }
}
