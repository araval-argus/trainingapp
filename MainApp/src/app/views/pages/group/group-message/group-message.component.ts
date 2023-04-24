import { Component, ElementRef, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { NgbModal, NgbModalConfig } from '@ng-bootstrap/ng-bootstrap';
import { GroupModel } from 'src/app/core/models/group-model';
import { GroupChatModel } from 'src/app/core/models/groupChat-model';
import { GroupNewChat } from 'src/app/core/models/groupNewChat-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { Profile } from 'src/app/core/models/profile-model';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';
import { GroupService } from 'src/app/core/service/group-service';
import { HubService } from 'src/app/core/service/hub-service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-group-message',
  templateUrl: './group-message.component.html',
  styleUrls: ['./group-message.component.scss']
})
export class GroupMessageComponent implements OnInit {
  //logged In User's data
  loggedInUser: LoggedInUser;
  //Thumbnail Image
  thumbnail;

  //To Toggle Emoji Bar
  isEmojiPickerVisible: boolean;

  //To get Information About Group
  selectedGroup: GroupModel = {
    name: '',
    description: '',
    admin: ''
  };

  //To manage media File
  url;
  format;
  unsafeUrl;

  //For New Message
  groupNewChat: GroupNewChat = {
    Content: '',
    GroupId: -1,
    Type: 'text'
  };
  //To get Value's of Chat
  @ViewChild('chatBar', { static: false }) chatBar: ElementRef


  //Chats To load
  chats: GroupChatModel[] = [];

  //replying Chat Id
  replyChatId: number = -1;
  replyingContent: string;


  //To manage Uploaded File
  file: File;

  //File Modal
  @ViewChild('fileModal', { static: false }) fileModal: TemplateRef<any>

  //To handle selected users
  selectedUsers = [];

  //already In group Memeber
  members = [];

  //To get current users
  users: Profile[] = [];


  //Group Details Update form
  editGroupDetail: FormGroup;
  editPhoto;
  editPhotoFile: File;


  //remove Member's
  removeMemberList = [];
  currentMembers: Profile[] = [];

  constructor(private groupService: GroupService, private authService: AuthService, private chatService: ChatService, private modalService: NgbModal, private domSanitizer: DomSanitizer, config: NgbModalConfig, private accountService: AccountService, private hubService: HubService, private formBuilder: FormBuilder) {
    config.backdrop = 'static';
    config.keyboard = false;
    config.centered = true;
  }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.isEmojiPickerVisible = false;
    this.groupService.groupSelection.subscribe((data: GroupModel) => {
      this.selectedGroup.id = data.id,
        this.selectedGroup.name = data.name,
        this.selectedGroup.description = data.description,
        this.selectedGroup.admin = data.admin
      this.thumbnail = data.profileImage != null ? this.setProfileImage(data.profileImage) : "https://via.placeholder.com/37x37";
      this.chats = [];
      this.reloadChat();
    })


    this.hubService.hubConnection.on("receivedChatForGroup", (data: any) => {
      if (data.groupId == this.selectedGroup.id) {
        this.pushMessage(data);
      }
    })


    this.hubService.hubConnection.on("addNotification", (msg) => {
      if (msg.groupId == this.selectedGroup.id) {
        this.pushMessage(msg);
      }
    })


    this.hubService.hubConnection.on("groupUpdate", data => {
      if (this.selectedGroup.id == data.id) {
        this.selectedGroup.description = data.description;
        this.thumbnail = data.profileImage != null ? this.setProfileImage(data.profileImage) : "https://via.placeholder.com/37x37";
      }
    })



    this.hubService.hubConnection.on("removedFromGroup", (data) => {
      if (this.selectedGroup.name == data) {
        this.selectedGroup.name = '';
      }
    })
  }


  //reload Chat 
  reloadChat() {
    this.cancelReply();
    this.chats = [];
    this.groupService.getAllChat(this.selectedGroup.name).subscribe((data: any) => {
      data.chats.forEach(element => {
        this.pushMessage(element);
      });
      this.chats.sort((a, b) => a.time.getTime() - b.time.getTime());
    })
  }

  //Reply feature TBI
  replyTo(id: number) {
    this.replyingContent = this.chats.find(e => e.Id == id).Content;
    this.replyChatId = id;
    this.chatBar.nativeElement.focus();
  }

  cancelReply() {
    this.replyChatId = -1;
    this.replyingContent = null;
  }

  //getFileURL
  getSrc(image: string) {
    return this.chatService.getFile(image);
  }

  //Checking format of the file
  checkFormat(type: string) {
    let format: string = '';
    if (type.indexOf('image') > -1) {
      format = 'image';
    } else if (type.indexOf('video') > -1) {
      format = 'video';
    } else if (type.indexOf('audio') > -1) {
      format = 'audio';
    }
    return format;
  }

  //Emoji
  addEmoji(event: any) {
    this.chatBar.nativeElement.value += event.emoji.native;
  }



  //TO manage File
  uploadedFile(event) {
    if (event.target.files.length > 0) {
      this.file = event.target.files[0];
    }
    this.convertFile();
    this.displayModal();

  }

  //Converting file to the url for modal
  convertFile() {
    var reader = new FileReader();
    reader.readAsDataURL(this.file);
    if (this.file.type.indexOf('image') > -1) {
      this.format = 'image';
    } else if (this.file.type.indexOf('video') > -1) {
      this.format = 'video';
    } else if (this.file.type.indexOf('audio') > -1) {
      this.format = 'audio';
    }
    reader.onload = (event) => {
      this.unsafeUrl = (<FileReader>event.target).result;
      this.url = this.domSanitizer.bypassSecurityTrustResourceUrl(this.unsafeUrl);
    }
  }

  //displaying modal
  displayModal() {
    this.modalService.open(this.fileModal, {}).result.then(() => {
    }).catch(() => {
    });
  }


  sendFile() {
    const formData: FormData = new FormData();
    formData.append('File', this.file);
    formData.append('GroupId', this.selectedGroup.id.toString());
    formData.append('RepliedId', this.replyChatId.toString());
    this.groupService.addFile(formData).subscribe((data: any) => {
      console.log(data);
    })
  }


  //On Enter Sent the Chat
  sendChat(event) {
    console.log("Called");

    event.preventDefault();
    const chat = this.chatBar.nativeElement.value;
    this.chatBar.nativeElement.value = "";
    if (chat.trim() != '') {
      this.groupNewChat = {
        Content: chat,
        GroupId: this.selectedGroup.id,
        RepliedId: this.replyChatId != -1 ? this.replyChatId : -1
      }
      this.groupService.addMessage(this.groupNewChat).subscribe((data: any) => {
        console.log(data);
      })
    }
    this.cancelReply()
  }

  //Convert File to URL
  setProfileImage(file: string) {
    return this.groupService.fetchImage(file);
  }

  //Add Chat To Array
  pushMessage(msg: any) {
    this.chats.push({
      Id: msg.id,
      Content: msg.content,
      GroupId: msg.groupId,
      Sender: msg.senderUserName,
      time: new Date(msg.sentAt),
      ReplyingContent: msg.replyingContent,
      ReplyingId: msg.replyingMessageId,
      Type: msg.type,
    })
  }


  resetFile() {
    this.file = null;
  }


  //Add Members
  //To handle submission event of adding member modal
  membersToAdd() {
    this.groupService.addMembers(this.selectedUsers, this.selectedGroup.name).subscribe(data => {
    })
    this.clearSelection();
  }

  //To clear selected List on closing of modal
  clearSelection() {

    this.selectedUsers = [];
    this.members = [];
    this.users = [];
  }


  //To add and Manage Members Of the Grou
  addMemberModal(content: TemplateRef<any>) {
    this.groupService.getMembers(this.selectedGroup.name).subscribe((data: any) => {
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
    this.modalService.open(content, {});
  }

  //About group Modal
  viewMemberList: any[] = [];
  aboutGroupModal(content: TemplateRef<any>) {
    this.modalService.open(content, {});
    this.groupService.getMembers(this.selectedGroup.name).subscribe((data: any) => {
      if (data.members != null) {
        data.members.forEach(element => {
          this.viewMemberList.push(element.userName + " -> " + element.firstName + " " + element.lastName);
        });
      }
    })
  }

  resetviewMemberList() {
    this.viewMemberList = [];
  }

  //UpdateGroup
  editGroupModal(content: TemplateRef<any>) {
    this.editGroupDetail = this.formBuilder.group({
      groupDesc: [this.selectedGroup.description],
      groupProfile: null
    })
    this.modalService.open(content, {});
    this.editPhoto = this.thumbnail;
  }

  editPhotoUploaded(event) {
    if (event.target.files.length > 0) {
      this.editPhotoFile = event.target.files[0];
      var reader = new FileReader();
      reader.readAsDataURL(this.editPhotoFile);
      reader.onload = (event) => {
        var fileResult = (<FileReader>event.target).result;
        this.editPhoto = this.domSanitizer.bypassSecurityTrustResourceUrl(fileResult.toString());
      }
    }

  }


  //submitEditFile
  submitUpdateForm() {
    console.log(this.editGroupDetail.value.groupProfile);
    const formData: FormData = new FormData();
    formData.append('Name', this.selectedGroup.name);
    formData.append('Description', this.editGroupDetail.value.groupDesc),
      formData.append('Image', this.editPhotoFile)
    this.groupService.updateGroup(formData).subscribe((data: any) => {
    })
  }

  //Remove Members
  removeMemberModal(content: TemplateRef<any>) {
    this.groupService.getMembers(this.selectedGroup.name).subscribe((data: any) => {
      console.log(data);
      this.currentMembers = data.members;
      this.currentMembers.splice(this.currentMembers.findIndex(e => e.userName == this.loggedInUser.sub), 1);
    });
    this.modalService.open(content, {});
  }


  clearRemoveList() {
    this.currentMembers = [];
    this.removeMemberList = [];
  }


  removeMember() {
    this.groupService.removeMember(this.removeMemberList, this.selectedGroup.name).subscribe((data: any) => {
      console.log(data);
      this.clearRemoveList();
    })
  }

  //Leave Group
  leaveGroup() {
    this.groupService.leaveGroup(this.selectedGroup.name).subscribe((data: any) => {
      console.log(data);
    })
  }
}
