import { Component, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GroupService } from 'src/app/core/service/group-service';
import { CreateGroupModel } from 'src/app/core/models/create-group-model';
import { RecentGroupModel } from 'src/app/core/models/recent-group-model';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/core/service/auth-service';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { ActivatedRoute, Router } from '@angular/router';
import { SignalRService } from 'src/app/core/service/signalr-service';
import { GroupModel } from 'src/app/core/models/group-model';
import { GroupMessageDisplayModel } from 'src/app/core/models/group-message-display-model';
import { AccountService } from 'src/app/core/service/account-service';

@Component({
  selector: 'app-group-sidebar',
  templateUrl: './group-sidebar.component.html',
  styleUrls: ['./group-sidebar.component.scss']
})
export class GroupSidebarComponent implements OnInit {

  url : any= '';
  loggedInUser: LoggedInUser;
  imageSource : string;
  imageFile : File;
  defaultNavActiveId: any;
  recentGroupList : RecentGroupModel[] = [];
  createGrpModel : CreateGroupModel;
  environment = environment.ImageUrl;

  constructor(private modalService: NgbModal , private groupService: GroupService , private authService:AuthService,
    private router : Router , private route : ActivatedRoute , private signalRService:SignalRService , private accountService:AccountService) { }

  ngOnInit(): void {

    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.imageSource = environment.ImageUrl + this.loggedInUser.imagePath;

    this.createGrpModel={
      groupName: '',
      description: '',
      imagePath: ''
    };

    this.groupService.loadRecentGroups().subscribe((data:RecentGroupModel[])=>{
      this.recentGroupList = data;
    });

    this.accountService.getUserStatus(this.loggedInUser.userName).subscribe((data:{id : number, status : string})=>{
      this.loggedInUser.status=data.status;
    });

    // To Remove groupId from groupList in which user pressed remove
    this.groupService.UserRemoveGroupSub.subscribe((groupId:number)=>{
      this.recentGroupList = this.recentGroupList.filter(group => group.id !== groupId);
    });

    this.signalRService.hubConnection.on('iAmAddedToGroup',(newGrp:RecentGroupModel)=>{
      this.recentGroupList.splice(0,0,newGrp);
    });

    this.signalRService.hubConnection.on('iAmRemovedFromGroup',(groupId:number)=>{
      this.recentGroupList = this.recentGroupList.filter(group => group.id !== groupId);
    });

    //To update List When Someone Updated Group
    this.signalRService.hubConnection.on('GroupUpdated',(group:GroupModel)=>{
      const index = this.recentGroupList.findIndex(u => u.id === group.id);
      if (index !== -1) {
        this.recentGroupList[index].groupImage = group.imagePath;
        this.recentGroupList[index].groupName = group.groupName;
      }
    });

    this.signalRService.hubConnection.on('RecieveMessageGroup',(data:GroupMessageDisplayModel)=>{
      this.groupService.loadRecentGroups().subscribe((data:RecentGroupModel[])=>{
        this.recentGroupList = data;
      });
    });
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

  onSubmit(){
    const formdata = new FormData();
    formdata.append('GroupName',this.createGrpModel.groupName);
    formdata.append('Description',this.createGrpModel.description);
    formdata.append('ImageFile',this.imageFile);
    this.groupService.createGroup(formdata).subscribe((data:RecentGroupModel)=>{
      this.recentGroupList.splice(0,0,data);
    })
  }

  onEditProfile(){
    this.router.navigate(['../update-profile'],{relativeTo:this.route});
   }

   onViewProfile(){
    this.router.navigate(['../view-profile'],{relativeTo:this.route});
   }

  openVerticalCenteredModal(content) {
    this.modalService.open(content, {centered: true}).result.then((result) => {
    }).catch((res) => {});
  }

  OnSelectGroup(groupId:number){
    this.groupService.GroupChangedSub.next(groupId);
  }

  recentGroupDate(date: Date): string {
    const recievedDate = new Date(date)
    const today = new Date();
    let curDate = recievedDate.getDate();
    let curMonth = recievedDate.getMonth();
    let curYear = recievedDate.getFullYear()
    if (
      curDate === today.getDate() &&
      curMonth === today.getMonth() &&
      curYear === today.getFullYear()
    ) {
      // Date is same as today, return time
      const hours = recievedDate.getHours().toString().padStart(2, '0');
      const minutes = recievedDate.getMinutes().toString().padStart(2, '0');
      return `${hours}:${minutes}`;
    } else {
      // Date is different from today, return full date
      return `${curDate}/${curMonth}/${curYear}`;
    }
  }

}
