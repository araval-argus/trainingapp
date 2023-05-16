import { AfterViewChecked, Component, ElementRef, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GroupMessageModel } from 'src/app/core/models/group-message-model';
import { GroupModel } from 'src/app/core/models/group-model';
import { LoggedInUserModel } from 'src/app/core/models/loggedin-user';
import { AuthService } from 'src/app/core/service/auth-service';
import { GroupService } from 'src/app/core/service/group.service';
import { SignalRService } from 'src/app/core/service/signalR-service';
import { environment } from 'src/environments/environment';
import { GroupMemberModel } from 'src/app/core/models/group-member-model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-group-content-body',
  templateUrl: './group-content-body.component.html',
  styleUrls: ['./group-content-body.component.scss']
})
export class GroupContentBodyComponent implements OnInit, AfterViewChecked, OnDestroy {

  selectedGroup: GroupModel;
  joinedUsers: GroupMemberModel[] = [];

  subscriptions: Subscription[] = [];
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
    this.subscribeToGroupSelected();
    this.subscribeToGroupMessageAdded();
    this.subscribeToNotificationRaised();
  }

  subscribeToNotificationRaised(){
    const sub = this.signalRService.notificationRaised.subscribe( notification => {
      if(notification.raisedInGroup !== this.selectedGroup.name){
        this.signalRService.addNotification.next(notification);
      }
    });
    this.subscriptions.push(sub);
  }

  subscribeToGroupMessageAdded(){
    const sub = this.signalRService.groupMessageAdded.subscribe((groupMessage: GroupMessageModel) => {
      if(groupMessage.groupId === this.selectedGroup.id){
        groupMessage.senderImage = this.joinedUsers.find( user => user.userName === groupMessage.senderUserName).imageUrl;
        groupMessage.message = this.setPath(groupMessage.messageType) + groupMessage.message;
        groupMessage.createdAt = new Date(groupMessage.createdAt)
        this.messages.push(groupMessage);
      }
    });
    this.subscriptions.push(sub);
  }

  subscribeToGroupSelected(){
    const sub = this.groupService.groupSelected.subscribe( (group: GroupModel) => {
      this.selectedGroup = group;
      this.fetchJoinedUsers(group.id);
      if(this.loggedInUser.sub && group){
        this.fetchMessages(this.loggedInUser.sub, group.id);
      }
    });
    this.subscriptions.push(sub);
  }

  fetchJoinedUsers(groupId){
    this.groupService.fetchJoinedUsers(groupId).subscribe( (users: GroupMemberModel[]) => {
      this.joinedUsers = users;
        this.joinedUsers.forEach((user) => {
          user.imageUrl =
            environment.apiUrl + "/../Images/Users/" + user.imageUrl;
        });
    })
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

  fetchMessages(userName: string, groupId: number){
    console.log("messages fetched");
    this.groupService
      .fetchMessages(userName, groupId)
      .subscribe((messages: GroupMessageModel[]) => {
        messages.forEach((message) => {
          message.senderImage = this.joinedUsers.find(
            (user) => user.userName === message.senderUserName
          ).imageUrl;
          message.message = this.setPath(message.messageType) + message.message;
          message.createdAt = new Date(message.createdAt + "Z");
        });
        this.messages = messages;
      });
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

  ngOnDestroy(){
    this.subscriptions.forEach( sub => sub.unsubscribe());
  }
}
