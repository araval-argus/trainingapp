import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Subscription } from 'rxjs';
import { GroupModel } from 'src/app/core/models/group-model';
import { LoggedInUserModel } from 'src/app/core/models/loggedin-user';
import { AuthService } from 'src/app/core/service/auth-service';
import { GroupService } from 'src/app/core/service/group.service';
import { SignalRService } from 'src/app/core/service/signalR-service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-groups-sidebar',
  templateUrl: './groups-sidebar.component.html',
  styleUrls: ['./groups-sidebar.component.scss']
})
export class GroupsSidebarComponent implements OnInit {

  groups: GroupModel[] = [];
  loggedInUser: LoggedInUserModel = this.authService.getLoggedInUserInfo();
  today = new Date();



  constructor(private authService: AuthService,
    private modalService: NgbModal,
    private groupService: GroupService,
    private signalRService: SignalRService) { }

  ngOnInit(): void {
    this.fetchGroups();
  }

  onClickGroup(group){
    this.groupService.groupSelected.next(group);
  }

  openScrollableModal(content) {
    this.modalService.open(content).result.then((result) => {
    }).catch((res) => {
      console.log(res)
    });

  }

  createGroup(){
    const groupName = (document.querySelector("#groupName") as HTMLInputElement).value;
    const groupDescription = (document.querySelector("#groupDescription") as HTMLInputElement).value;
    const groupIcon = (document.querySelector("#groupIcon") as HTMLInputElement).files[0];

    const formData = new FormData();
    formData.append("name", groupName);
    formData.append("description", groupDescription);
    formData.append("groupIcon", groupIcon);
    formData.append("creatorUserName", this.loggedInUser.sub);

    this.groupService.createGroup(formData).subscribe( (group: GroupModel) => {
      if(group.groupIconUrl){
        group.groupIconUrl = environment.apiUrl + "/../GroupIcons/" + group.groupIconUrl;
      }
      else{
        group.groupIconUrl = environment.apiUrl + "/../Default/default_group_icon.jpg";
      }
      group.lastMessageTimeStamp = new Date();
      this.groups.splice(0,0,group);
      console.log(group);
    },err=>{
      console.log(err)
    })
  }

  fetchGroups(){
    if(this.loggedInUser.sub){
      this.groupService.fetchGroups(this.loggedInUser.sub).subscribe( (data: any) => {
        this.groups = data;
        this.groups.forEach((group) => {
          if (group.groupIconUrl) {
            group.groupIconUrl =
              environment.apiUrl + "/../GroupIcons/" + group.groupIconUrl;
          }
          else{
            group.groupIconUrl =
            environment.apiUrl + "/../Default/default_group_icon.jpg";
          }
          group.lastMessageTimeStamp = new Date(group.lastMessageTimeStamp + 'Z');
        });
      });
    }
  }


}
