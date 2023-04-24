import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { NgbModal, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';
import { GroupModel } from 'src/app/core/models/group-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import { GroupService } from 'src/app/core/service/group-service';
import { HubService } from 'src/app/core/service/hub-service';

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

  //To add Group data to the formData
  formData: FormData = new FormData();
  //GroupList
  groupList: GroupModel[] = [];




  constructor(config: NgbModalConfig, private modalService: NgbModal, private domSanitizer: DomSanitizer, private accountService: AccountService, private groupService: GroupService, private authService: AuthService, private hubService: HubService) {
    config.backdrop = 'static';
    config.keyboard = false;
    config.centered = true;
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


    this.groupService.openFromNotification.subscribe(data => {
      let group = this.groupList.find(e => e.name == data);
      if (group != null) {
        this.selectedGroup(group);
      }
    })

    this.hubService.hubConnection.on("receivedChatForGroup", (data: any) => {
      console.log(data);
    })

    this.hubService.hubConnection.on("addedToGroup", (data: any) => {
      this.groupList.push(data);
    })

    this.hubService.hubConnection.on("groupUpdate", data => {
      var groupIndex = this.groupList.findIndex(e => e.id == data.id);
      if (groupIndex != -1) {
        this.groupList.splice(groupIndex, 1);
        this.groupList.push({
          id: data.Id,
          name: data.name,
          description: data.description,
          profileImage: data.profileImage,
          admin: data.admin
        });
      }
    })

    this.hubService.hubConnection.on("removedFromGroup", (data) => {
      this.groupList = this.groupList.filter(e => e.name != data);
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
    this.groupService.addGroup(this.formData).subscribe((data: any) => {
      this.groupList.push(data.savedGroup);
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

  //OnSelection Of Group
  selectedGroup(group: GroupModel) {
    this.groupService.groupSelection.next(group);
  }
}
