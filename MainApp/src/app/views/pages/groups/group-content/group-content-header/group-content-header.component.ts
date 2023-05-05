import { Component, Input, OnChanges, OnInit } from "@angular/core";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { GroupModel } from "src/app/core/models/group-model";
import { LoggedInUserModel } from 'src/app/core/models/loggedin-user';
import { AuthService } from 'src/app/core/service/auth-service';
import { GroupService } from "src/app/core/service/group.service";
import { environment } from "src/environments/environment";
import Swal from 'sweetalert2';
import { GroupMemberModel } from 'src/app/core/models/group-member-model';
import { SignalRService } from 'src/app/core/service/signalR-service';

@Component({
  selector: "app-group-content-header",
  templateUrl: "./group-content-header.component.html",
  styleUrls: ["./group-content-header.component.scss"],
})
export class GroupContentHeaderComponent implements OnInit, OnChanges {
  @Input() selectedGroup: GroupModel;
  @Input() joinedUsers: GroupMemberModel[];

  loggedInUser: LoggedInUserModel;
  notJoinedUsers: GroupMemberModel[];
  groupIcon: File;

  editGroupModel: GroupModel;

  constructor(
    private modalService: NgbModal,
    private groupService: GroupService,
    private authService: AuthService,
    private signalRService: SignalRService
  ) {}

  ngOnChanges(){

    if (this.selectedGroup.loggedInUserIsAdmin) {
      this.groupService
        .fetchNotJoinedUsers(this.selectedGroup.id)
        .subscribe((data: any) => {
          this.notJoinedUsers = data;
          this.notJoinedUsers.forEach((user) => {
            user.imageUrl =
              environment.apiUrl + "/../Images/Users/" + user.imageUrl;
          });
        });
    }

    this.editGroupModel = {...this.selectedGroup};
  }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.groupService.fetchJoinedUsers(this.selectedGroup.id).subscribe( (users: GroupMemberModel[]) => {
      this.joinedUsers = users;
      this.joinedUsers.forEach((user) => {
        user.imageUrl =
          environment.apiUrl + "/../Images/Users/" + user.imageUrl;
      });
      this.signalRService.memberRemoved.subscribe( (memberUserName: string) => {
        if(this.joinedUsers){
          let member = this.joinedUsers.find(member => member.userName === memberUserName)
          this.joinedUsers.splice(this.joinedUsers.indexOf(member), 1);
          if (this.selectedGroup.loggedInUserIsAdmin){
            this.notJoinedUsers.push(member);
          }
        }
      })
    })
    if (this.selectedGroup.loggedInUserIsAdmin) {
      this.groupService
        .fetchNotJoinedUsers(this.selectedGroup.id)
        .subscribe((data: any) => {
          this.notJoinedUsers = data;
          this.notJoinedUsers.forEach((user) => {
            user.imageUrl =
              environment.apiUrl + "/../Images/Users/" + user.imageUrl;
          });
          //console.log(this.notJoinedUsers)
        });

    }
    //console.log(this.loggedInUser);

    this.editGroupModel = {...this.selectedGroup};
  }

  openScrollableModal(content) {
    this.modalService
      .open(content, { scrollable: true })
      .result.then((result) => {})
      .catch((res) => {});
  }

  openBasicModal(content) {
    this.modalService
      .open(content)
      .result.then((result) => {})
      .catch((res) => {});
  }


  addMember(user){
    this.notJoinedUsers.splice(this.notJoinedUsers.indexOf(user),1);
    this.joinedUsers.push(user);
    this.groupService.addMember(this.selectedGroup.id, user).subscribe( data => {
      //console.log(data);
    })
  }

  leaveGroup(memberUserName: string){
    Swal.fire({
      title: 'Are you sure?',
      text: "Do you want to leave this group?",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Leave'
    }).then((result) => {
      if (result.isConfirmed) {
        this.groupService.removeMember(memberUserName,this.selectedGroup.id).subscribe( (data: any) => {
          console.log(data);
          Swal.fire(
            'Group left',
            data.message,
            'success'
          )
        }, err => {
          Swal.fire(
            'Error!!',
            err,
            'error'
          )
          console.log(err);
        });

        this.groupService.groupLeft.next(this.selectedGroup);

      }
    })
  }

  updateGroup(){
    console.log("üpdate group")
    let formData = new FormData();
    formData.append("id", this.editGroupModel.id+"");
    formData.append("name", this.editGroupModel.name);
    formData.append("description", this.editGroupModel.description);
    formData.append("creatorUserName", this.editGroupModel.creatorUserName);
    if(this.groupIcon){
      formData.append("groupIcon", this.groupIcon);
    }
    this.groupService.updateGroup(formData).subscribe( (group: GroupModel) => {
      this.groupService.selectedGroupDetailsUpdated.next(group);
    })
  }

  iconChanged(event: Event){
    this.groupIcon = (event.target as HTMLInputElement).files[0];

    var reader = new FileReader();
    reader.onload = (e) => {
      this.editGroupModel.groupIconUrl = e.target.result as string;
    };
    if(this.groupIcon){
      console.log("inside if")
      reader.readAsDataURL(this.groupIcon);
    }
  }

  removeMember(member: GroupMemberModel){
    this.groupService.removeMember(member.userName, this.selectedGroup.id).subscribe( data => {
      console.log(data);
      this.notJoinedUsers.push(member);
      this.joinedUsers.splice(this.joinedUsers.indexOf(member), 1);
    }, err => {
      console.log(err);
    })
  }

  makeAdmin(member: GroupMemberModel){
    this.groupService.makeAdmin(member).subscribe( (groupMember: GroupMemberModel) => {
      const index = this.joinedUsers.indexOf(member);
      this.joinedUsers.splice(index, 1);
      this.joinedUsers.splice(index, 0, groupMember);
    })
  }
}
