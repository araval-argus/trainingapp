import { AfterViewChecked, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AllGroupMember } from 'src/app/core/models/all-group-member.model';
import { AllUserModel } from 'src/app/core/models/all-user-model';
import { GroupMessageDisplayModel } from 'src/app/core/models/group-message-display-model';
import { GroupMessageModel } from 'src/app/core/models/group-message-model';
import { GroupModel } from 'src/app/core/models/group-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { AuthService } from 'src/app/core/service/auth-service';
import { GroupService } from 'src/app/core/service/group-service';
import { SignalRService } from 'src/app/core/service/signalr-service';
import { environment } from 'src/environments/environment';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-group-load',
  templateUrl: './group-load.component.html',
  styleUrls: ['./group-load.component.scss']
})
export class GroupLoadComponent implements OnInit , AfterViewChecked , OnDestroy {

  constructor(private groupService : GroupService , private route: ActivatedRoute , private modalService : NgbModal, private authService:AuthService,
    private router:Router , private signalRService:SignalRService) { }

  groupId : number ;
  imageFile : File;
  loggedInUser : LoggedInUser;
  url: any = '';
  isAdmin : number = 0;
  groupDetail : GroupModel;
  allUsers : AllGroupMember[] = [];
  selectedUsers = [];
  environment = environment.ImageUrl;
  members : AllGroupMember[] = [];
  displayMsgList : GroupMessageDisplayModel[] = [];
  isReplying : Boolean = false;
  rplyMsg : string ;
  msg : GroupMessageModel;
  rplyMsgLen : number;
  rplyMsgId: number;
  showEmoji : boolean = false;
  @ViewChild('scrollMe') private myScrollContainer: ElementRef;

  ngOnInit(): void {
  this.groupService.GroupChangedSub.subscribe((groupId:number)=> {
   this.groupId = groupId;

   this.loggedInUser = this.authService.getLoggedInUserInfo();

    this.signalRService.hubConnection.on('MadeMeAdmin',(groupId:number)=>{
        if(this.groupId==groupId){
           this.isAdmin=1;
        }
    })

    this.msg = {
      content: '',
      messageFrom: '',
      groupId: this.groupId,
      repliedTo:-1,
      type:''
    };

    this.groupService.fetchMessages(this.groupId).subscribe((data:GroupMessageDisplayModel[])=>{
      this.displayMsgList = data;
    });

    this.groupService.getGroup(this.groupId).subscribe((data:GroupModel)=>{
      this.groupDetail = data;
    });

    this.groupService.getAllMembers(this.groupId).subscribe((data:AllGroupMember[])=>{
      this.members = data;
      var curUserMember = this.members.find(u=>u.userName==this.loggedInUser.userName);
      this.isAdmin = curUserMember.admin;
    });
    });

    this.signalRService.hubConnection.on('GroupUpdated',(group:GroupModel)=>{
      this.groupDetail=group;
    });

    this.signalRService.hubConnection.on('RecieveMessageGroup',(data:GroupMessageDisplayModel)=>{
        this.displayMsgList.push(data);
    });

    this.signalRService.hubConnection.on('iAmRemovedFromGroup',(groupId:number)=>{
      if(this.groupId==groupId){
        this.groupDetail=null;
      }
    });

    this.scrollToBottom();
  }

  ngAfterViewChecked():void {
    this.scrollToBottom();
  }

  onSubmit(){
    if(this.selectedUsers!=null){
      this.groupService.addMemberToGroup(this.selectedUsers,this.groupId).subscribe((data:AllGroupMember[])=>{
        data.forEach(element=>{
          this.members.push(element);
        })
      });
      this.selectedUsers = [];
    }
  }

  onMessage(message:HTMLInputElement){
    if(message.value.trim()==''){}
    else{
    this.msg.content = message.value;
    this.msg.messageFrom = this.loggedInUser.userName;
    this.msg.groupId = this.groupId;
    if(this.isReplying)
    {
      this.msg.repliedTo = this.rplyMsgId;
    }
    else{
      this.msg.repliedTo = -1;
    }
    this.msg.type=null;
    this.signalRService.hubConnection.invoke('sendGroupMsg',this.msg).catch((error)=>console.log(error));
    message.value = null;
    this.closeRplyMsg();
    }
  }

  onEdit(){
    const formdata = new FormData();
    formdata.append('GroupName',this.groupDetail.groupName);
    formdata.append('Description',this.groupDetail.description);
    formdata.append('ImageFile',this.imageFile);
    this.groupService.editGroup(this.groupId,formdata).subscribe(()=>{})
    this.url='';
    this.imageFile=null;
  }

  onSelectFile(event) {
    if (event.target.files && event.target.files[0]) {
      var reader = new FileReader();

      if (event.target.files.length > 0) {
        this.imageFile = (event.target as HTMLInputElement).files[0];}

      reader.readAsDataURL(event.target.files[0]); // read file as data url

      reader.onload = (event) => { // called once readAsDataURL is completed
        this.url = event.target.result;
      }
    }
  }

  onExit(){
    Swal.fire({
      title: 'Are you sure?',
      text: "You won't be able to revert this!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Yes, Remove me!'
    }).then((result) => {
      if (result.isConfirmed) {
        this.groupService.leaveGroup(this.groupId).subscribe(()=>{
          this.groupService.UserRemoveGroupSub.next(this.groupId);
          this.groupDetail=null;
        });
        Swal.fire(
          'Removed!',
          'You Have Exited From Group Successfully',
          'success'
        )
        this.router.navigate(['../'], { relativeTo: this.route });
      }
    })
  }

  makeAdmin(userName:string){
    if(this.isAdmin==1){
    const index = this.members.findIndex(member => member.userName === userName);
    if (index !== -1) {
      this.members[index].admin = 1;
    }
    this.groupService.makeUserasAdmin(this.groupId,userName).subscribe();
   }
  }

  removeUser(userName:string){
    if(this.isAdmin==1){
      this.members = this.members.filter(group => group.userName !== userName);
      this.groupService.removeUserFromGroup(this.groupId,userName).subscribe();
     }
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
    formdata.append('GroupId',''+this.groupId),
    this.groupService.sendFileMessage(formdata).subscribe(()=>{})
  }

  toggleReplyMsg(popupmessage:GroupMessageDisplayModel){
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

  onCloseEdit(){
    this.url='';
    this.imageFile=null;
  }

  NavigateToChat(userName:string){
    if(userName!=this.loggedInUser.userName){
    this.router.navigate(['../../chat/'+userName],{relativeTo:this.route});
    }
  }

  openVerticalCenteredModal(content) {
    //Fetch All Users When Clicked on Add Members
    this.groupService.getAllUsers(this.groupId).subscribe((data:AllUserModel[])=>{
      this.allUsers = data ;
    });
    this.modalService.open(content, {centered: true}).result.then((result) => {
    }).catch((res) => {});
  }

  openEditGroupModal(content){
    this.modalService.open(content, {centered: true}).result.then((result) => {
    }).catch((res) => {});
  }

  openScrollableModal(content) {
    this.groupService.getAllMembers(this.groupId).subscribe((data:AllGroupMember[])=>{
      this.members = data;
     });
    this.modalService.open(content, {scrollable: true}).result.then((result) => {
    }).catch((res) => {});
  }

  openBasicModal(content) {
    this.modalService.open(content, {}).result.then((result) => {
    }).catch((res) => {});
  }

  scrollToBottom(): void {
    try {
        this.myScrollContainer.nativeElement.scrollTop = this.myScrollContainer.nativeElement.scrollHeight;
    } catch(err) { }
  }

  Emoji(event, message : HTMLInputElement){
    const text = message.value + event.emoji.native;
    message.value = text;
    this.showEmoji = false;
  }

  toggleEmoji(message : HTMLInputElement){
    this.showEmoji = !this.showEmoji;
  }

  ngOnDestroy(): void {
      this.groupDetail=null;
  }
}
