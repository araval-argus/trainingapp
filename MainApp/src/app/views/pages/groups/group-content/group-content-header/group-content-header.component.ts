import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output } from "@angular/core";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { GroupModel } from "src/app/core/models/group-model";
import { LoggedInUserModel } from 'src/app/core/models/loggedin-user';
import { AuthService } from 'src/app/core/service/auth-service';
import { GroupService } from "src/app/core/service/group.service";
import { environment } from "src/environments/environment";
import Swal from 'sweetalert2';
import { GroupMemberModel } from 'src/app/core/models/group-member-model';
import { SignalRService } from 'src/app/core/service/signalR-service';
import { Subscription } from 'rxjs';
import { take } from 'rxjs/operators';

@Component({
  selector: "app-group-content-header",
  templateUrl: "./group-content-header.component.html",
  styleUrls: ["./group-content-header.component.scss"],
})
export class GroupContentHeaderComponent implements OnInit, OnChanges, OnDestroy {
  @Input() selectedGroup: GroupModel;
  @Input() joinedUsers: GroupMemberModel[];

  loggedInUser: LoggedInUserModel;
  notJoinedUsers: GroupMemberModel[] = [];
  subscriptions: Subscription[] = [];
  groupIcon: File;
  editGroupModel: GroupModel;

  constructor(
    private modalService: NgbModal,
    private groupService: GroupService,
    private authService: AuthService,
    private signalRService: SignalRService
  ) {}

  ngOnChanges(){
  }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.subscribeToFetchJoinedUsers();
    this.subscribeToFetchNotJoinedUsers();
    this.subscribeToNewMemberAdded();
  }

  subscribeToFetchJoinedUsers(){
    const sub = this.groupService.fetchJoinedUsers(this.selectedGroup.id).subscribe( (users: GroupMemberModel[]) => {
      this.joinedUsers = users;
      this.joinedUsers.forEach((user) => {
        user.imageUrl =
          environment.apiUrl + "/../Images/Users/" + user.imageUrl;
      });
      this.subscribeToMemberRemoved();
    })
    this.subscriptions.push(sub);
  }

  subscribeToMemberRemoved(){
    const sub = this.signalRService.memberRemoved.subscribe( (member: GroupMemberModel) => {
      if(this.joinedUsers){
        member = this.joinedUsers.find( m => m.userName == member.userName);
        if(member){
          this.joinedUsers.splice(this.joinedUsers.indexOf(member), 1);
        }
        if (this.selectedGroup.loggedInUserIsAdmin){
          this.notJoinedUsers.push(member);
        }
      }
    })
    this.subscriptions.push(sub);
  }

  subscribeToFetchNotJoinedUsers(){
    if (this.selectedGroup.loggedInUserIsAdmin) {
      const sub = this.groupService
        .fetchNotJoinedUsers(this.selectedGroup.id)
        .subscribe((data: any) => {
          this.notJoinedUsers = data;
          this.notJoinedUsers.forEach((user) => {
            user.imageUrl =
              environment.apiUrl + "/../Images/Users/" + user.imageUrl;
          });
        });
        this.subscriptions.push(sub);
    }
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


  addMember(user: GroupMemberModel){
    this.groupService.addMember(this.selectedGroup.id, user).pipe(take(1)).subscribe( data => {

    })
  }



  updateGroup(){
    let formData = new FormData();
    formData.append("id", this.editGroupModel.id+"");
    formData.append("name", this.editGroupModel.name);
    formData.append("description", this.editGroupModel.description);
    formData.append("creatorUserName", this.editGroupModel.creatorUserName);
    if(this.groupIcon){
      formData.append("groupIcon", this.groupIcon);
    }
    const sub = this.groupService.updateGroup(formData).subscribe( (group: GroupModel) => {
      this.groupService.selectedGroupDetailsUpdated.next(group);
    })
    sub.unsubscribe();
  }

  iconChanged(event: Event){
    this.groupIcon = (event.target as HTMLInputElement).files[0];

    var reader = new FileReader();
    reader.onload = (e) => {
      this.editGroupModel.groupIconUrl = e.target.result as string;
    };
    if(this.groupIcon){
      reader.readAsDataURL(this.groupIcon);
    }
  }

  removeMember(member: GroupMemberModel){
    this.groupService.removeMember(member.userName, this.selectedGroup.id).pipe(take(1)).subscribe( data => {

    }, err => {
      console.log(err);
    });
  }

  makeAdmin(member: GroupMemberModel){
    const sub = this.groupService.makeAdmin(member).subscribe( (groupMember: GroupMemberModel) => {
      const index = this.joinedUsers.indexOf(member);
      this.joinedUsers.splice(index, 1);
      this.joinedUsers.splice(index, 0, groupMember);
    })
    sub.unsubscribe();
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
        this.groupService.removeMember(memberUserName,this.selectedGroup.id).pipe(take(1)).subscribe( (data: any) => {
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

 subscribeToNewMemberAdded(){
    const sub = this.signalRService.newMemberAdded.subscribe( (newMember: GroupMemberModel) => {
      this.notJoinedUsers.splice(this.notJoinedUsers.indexOf(newMember), 1);
      if(this.joinedUsers){
        this.joinedUsers.push(newMember);
      }
    });
    this.subscriptions.push(sub);
  }

  ngOnDestroy(){
    this.subscriptions.forEach( sub => sub.unsubscribe());
  }
}
