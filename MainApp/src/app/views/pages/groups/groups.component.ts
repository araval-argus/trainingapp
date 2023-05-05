import { Component, OnInit } from '@angular/core';
import { GroupModel } from 'src/app/core/models/group-model';
import { LoggedInUserModel } from 'src/app/core/models/loggedin-user';
import { AuthService } from 'src/app/core/service/auth-service';
import { GroupService } from 'src/app/core/service/group.service';
import { environment } from 'src/environments/environment';
import Swal from 'sweetalert2';
import { SignalRService } from 'src/app/core/service/signalR-service';
import { GroupMessageModel } from 'src/app/core/models/group-message-model';
import { GroupMemberModel } from 'src/app/core/models/group-member-model';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.component.html',
  styleUrls: ['./groups.component.scss']
})
export class GroupsComponent implements OnInit {

  loggedInUser: LoggedInUserModel;

  groups: GroupModel[];

  selectedGroup: GroupModel;

  joinedUsers: GroupMemberModel[];

  constructor(private authService: AuthService,
    private groupService: GroupService,
    private signalRService: SignalRService) { }

  ngOnInit(): void {

    this.loggedInUser = this.authService.getLoggedInUserInfo();

    this.groupService
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

    this.groupService.groupSelected.subscribe( (group: GroupModel) => {
      this.selectedGroup = group;
      this.groupService.fetchJoinedUsers(this.selectedGroup.id).subscribe( (users: GroupMemberModel[]) => {
        this.joinedUsers = users;
        this.joinedUsers.forEach((user) => {
          user.imageUrl =
            environment.apiUrl + "/../Images/Users/" + user.imageUrl;
        });
      });
    });

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

    this.groupService.selectedGroupDetailsUpdated.subscribe( group => {

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

    //for shifting the group to top of the list when new msg comes
    this.signalRService.groupMessageAdded.subscribe( (groupMessageModel: GroupMessageModel) => {
      const group = this.groups.find( group => group.id === groupMessageModel.groupId);
      this.groups.splice(this.groups.indexOf(group),1);
      group.lastMessage = groupMessageModel.message;
      group.lastMessageTimeStamp = new Date(groupMessageModel.createdAt);
      this.groups.splice(0,0,group);
    });

    //for inserting a new group when the logged in user is added into it
    this.signalRService.newGroupAdded.subscribe( (groupId: number) => {

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

    //If the logged in user gets removed from the group
    this.signalRService.groupRemoved.subscribe((memberUserName: string) => {
      Swal.fire({
        icon: "error",
        title: "Oops...",
        text: "You were removed from the group!",
      });
      this.groups.splice(this.groups.indexOf(this.selectedGroup), 1);
      this.selectedGroup = null;
    });

    this.signalRService.newMemberAdded.subscribe( (newMember: GroupMemberModel) => {
      if(this.joinedUsers){
        this.joinedUsers.push(newMember);
      }
    });

    this.signalRService.memberRemoved.subscribe( (memberUserName: string) => {
      if(this.joinedUsers){
        let member = this.joinedUsers.find(member => member.userName === memberUserName)
        this.joinedUsers.splice(this.joinedUsers.indexOf(member), 1);
      }
    })
  }

}
