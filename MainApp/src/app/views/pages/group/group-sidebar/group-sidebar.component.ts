import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { NgbModal, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';
import { GroupModel } from 'src/app/core/models/group-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { Profile } from 'src/app/core/models/profile-model';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import { GroupService } from 'src/app/core/service/group-service';

@Component({
  selector: 'app-group-sidebar',
  templateUrl: './group-sidebar.component.html',
  styleUrls: ['./group-sidebar.component.scss'],
  providers: [NgbModalConfig, NgbModal]
})
export class GroupSidebarComponent implements OnInit {
  //To get loggedInUser
  loggedInUser: LoggedInUser
  //Below all variable is declared to manage create group form
  //Form Group For adding group Form
  addForm: FormGroup;
  //src path for uploaded image file of group profile image
  src = null;
  //To handle url created from uploaded file
  unsafeUrl;
  //To handle Uploaded Image
  uploadFile: File;
  //To get current users
  users: Profile[] = [];
  //To handle selected users
  selectedUsers = [];
  //already In group Memeber
  members = [];
  //To add Group data to the formData
  formData: FormData = new FormData();
  //GroupList
  groupList: GroupModel[] = [];
  //For adding Member Modal
  addMemberModalGroup: GroupModel = {
    name: '',
    description: ''
  }




  constructor(config: NgbModalConfig, private modalService: NgbModal, private domSanitizer: DomSanitizer, private accountService: AccountService, private groupService: GroupService, private authService: AuthService) {
    config.backdrop = 'static';
    config.keyboard = false;
  }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    //Initialize Groups in which user is added or is admin
    this.groupService.getAllGroup().subscribe((data: any) => {
      this.groupList = data.groups;
    })
    this.addForm = new FormGroup({
      groupName: new FormControl('', [Validators.required, Validators.maxLength(50)]),
      description: new FormControl('', Validators.maxLength(200)),
      groupProfile: new FormControl(''),
    })
  }


  //This will open add modal
  openAddModal(content: TemplateRef<any>) {
    this.modalService.open(content, {});
  }



  //onSubmit Of add Modal
  onSubmit() {
    this.formData.append('Name', this.addForm.value.groupName);
    this.formData.append('Description', this.addForm.value.description);
    if (this.uploadFile != null) {
      this.formData.append('Image', this.uploadFile);
    }
    this.groupService.addGroup(this.formData).subscribe(data => {
    }, err => {
    })
    this.clearForm();
  }

  //On Cancel delete the old data
  //This event will discard the form value, so that when new modal generate old value shouldn't be displayed
  clearForm() {
    this.addForm.reset();
    this.src = null;
  }

  //Setting up uploaded file as profile photo on add form modal
  uploadedFile(event) {
    if (event.target.files.length > 0) {
      this.uploadFile = event.target.files[0];
      var reader = new FileReader();
      reader.readAsDataURL(this.uploadFile);
      reader.onload = (event) => {
        this.unsafeUrl = (<FileReader>event.target).result;
        this.src = this.domSanitizer.bypassSecurityTrustResourceUrl(this.unsafeUrl);
      }
    }
  }


  //Get Image From Image Name
  getImage(imageName: string) {
    if (imageName == null) {
      return null;
    }
    return this.accountService.fetchImage(imageName);
  }

  //To add and Manage Members Of the Grou
  addMemberModal(groupName: string, content: TemplateRef<any>) {
    this.groupService.getMembers(groupName).subscribe((data: any) => {
      if (data.members != null) {
        data.members.forEach(element => {
          this.members.push(element.userName);
        });
      }
      this.accountService.getAll().subscribe((data: any) => {
        if (data != null) {
          this.users = data;
          this.members.forEach(element => {
            var userProfileIndex = this.users.findIndex(e => e.userName === element);
            if (userProfileIndex > -1) {
              this.users.splice(userProfileIndex, 1);
            }
          });
        }
      })
    })

    this.addMemberModalGroup.name = groupName;
    this.modalService.open(content, {});
  }


  //To handle submission event of adding member modal
  membersToAdd() {
    this.groupService.addMembers(this.selectedUsers, this.addMemberModalGroup.name).subscribe(data => {
    })
    this.clearSelection();
  }

  //To clear selected List on closing of modal
  clearSelection() {

    this.selectedUsers = [];
    this.members = [];
    this.users = [];
  }


  //OnSelection Of Group
  selectedGroup(event: Event, group: GroupModel) {
    event.stopPropagation();
    this.groupService.groupSelection.next(group);
  }
}
