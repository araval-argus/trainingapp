import { Component, OnDestroy, OnInit } from '@angular/core';
import { GroupModel } from 'src/app/core/models/group-model';
import { LoggedInUserModel } from 'src/app/core/models/loggedin-user';
import { AuthService } from 'src/app/core/service/auth-service';
import { GroupService } from 'src/app/core/service/group.service';
import { environment } from 'src/environments/environment';
import Swal from 'sweetalert2';
import { SignalRService } from 'src/app/core/service/signalR-service';
import { GroupMessageModel } from 'src/app/core/models/group-message-model';
import { GroupMemberModel } from 'src/app/core/models/group-member-model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.component.html',
  styleUrls: ['./groups.component.scss']
})
export class GroupsComponent implements OnInit, OnDestroy {

  loggedInUser: LoggedInUserModel;

  groups: GroupModel[];

  selectedGroup: GroupModel;

  joinedUsers: GroupMemberModel[];
  subscriptions: Subscription[] = [];

  constructor(private authService: AuthService,
    private groupService: GroupService,
    private signalRService: SignalRService) { }

  ngOnInit(): void {

    this.loggedInUser = this.authService.getLoggedInUserInfo();

    this.subscribeToFetchGroups();
    this.subscribeToGroupSelected();
    this.subscribeToGroupLeft();
    this.subscribeToSelectedGroupDetailsUpdated();
    //for shifting the group to top of the list when new msg comes
    this.subscribeToGroupMessageAdded();
    //for inserting a new group when the logged in user is added into it
    this.subscribeToNewGroupAdded();
    //If the logged in user gets removed from the group
    this.subscribeToGroupRemoved();
  }

  subscribeToFetchGroups(){
    const sub = this.groupService
    .fetchGroups(this.loggedInUser.sub)
    .subscribe((data: GroupModel[]) => {
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
    this.subscriptions.push(sub);
  }

  subscribeToGroupSelected(){
    const sub = this.groupService.groupSelected.subscribe( (group: GroupModel) => {
      if(group){
        this.selectedGroup = group;
      const sub = this.groupService.fetchJoinedUsers(this.selectedGroup.id).subscribe( (users: GroupMemberModel[]) => {
        this.joinedUsers = users;
        this.joinedUsers.forEach((user) => {
          user.imageUrl =
            environment.apiUrl + "/../Images/Users/" + user.imageUrl;
        });
        this.subscriptions.push(sub);
      });
      this.signalRService.notificationRemoved.next(group.name);
      }

    });
    this.subscriptions.push(sub);
  }

  subscribeToGroupLeft(){
    this.groupService.groupLeft.subscribe( group =>{
      this.groups.splice(this.groups.indexOf(group), 1);
      this.selectedGroup = null;

      Swal.fire({
        icon: "success",
        text: "Group left successfully",
        timer: 1500,
        timerProgressBar: true
      });
    });
  }

  subscribeToSelectedGroupDetailsUpdated(){
    const sub = this.groupService.selectedGroupDetailsUpdated.subscribe( group => {

      const indexOfGroup = this.groups.indexOf(this.selectedGroup);
      this.groups.splice(indexOfGroup, 1);

      if (group.groupIconUrl) {
        group.groupIconUrl =
          environment.apiUrl + "/../GroupIcons/" + group.groupIconUrl;
      }
      else{
        group.groupIconUrl =
        environment.apiUrl + "/../Default/default_group_icon.jpg";
      }
      this.selectedGroup = group;
      this.selectedGroup.lastMessageTimeStamp = new Date();

      this.groups.splice(indexOfGroup, 0, this.selectedGroup);
    });
    this.subscriptions.push(sub);
  }

  subscribeToGroupMessageAdded(){
    const sub = this.signalRService.groupMessageAdded.subscribe( (groupMessageModel: GroupMessageModel) => {
      const group = this.groups.find( group => group.id === groupMessageModel.groupId);
      this.groups.splice(this.groups.indexOf(group),1);
      group.lastMessage = groupMessageModel.message;
      group.lastMessageTimeStamp = new Date(groupMessageModel.createdAt);
      this.groups.splice(0,0,group);
    });
    this.subscriptions.push(sub);
  }

  subscribeToNewGroupAdded(){
    const sub = this.signalRService.newGroupAdded.subscribe( (groupId: number) => {

      this.groupService.fetchGroup(groupId).subscribe( (group: GroupModel) => {
        if(group.groupIconUrl){
          group.groupIconUrl = environment.apiUrl + "/../GroupIcons/" + group.groupIconUrl;
        }
        else{
          group.groupIconUrl = environment.apiUrl + "/../Default/default_group_icon.jpg";
        }
        group.lastMessageTimeStamp = new Date();
        this.groups.splice(0,0, group)
      })
    });

    this.subscriptions.push(sub);
  }

  subscribeToGroupRemoved(){
    const sub = this.signalRService.groupRemoved.subscribe((memberUserName: string) => {
      Swal.fire({
        icon: "error",
        title: "Oops...",
        text: "You were removed from the group!",
      });
      this.groups.splice(this.groups.indexOf(this.selectedGroup), 1);
      this.selectedGroup = null;
    });
    this.subscriptions.push(sub);
  }

  ngOnDestroy(){
    this.subscriptions.forEach( sub => sub.unsubscribe());
  }
}
