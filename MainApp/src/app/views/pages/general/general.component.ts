import { Component, OnInit, TemplateRef } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { LoggedInUser } from 'src/app/core/models/user/loggedin-user';
import { GeneralService } from 'src/app/core/service/general.service';
import { UserService } from 'src/app/core/service/user.service';

@Component({
  selector: 'app-general',
  templateUrl: './general.component.html',
  styleUrls: ['./general.component.scss']
})
export class GeneralComponent implements OnInit {

  user: LoggedInUser;
  areas:{id:number,name:string}[] = [];
  areaId:number|null=null;
    post:{
    payPerHour:number,
    role:string,
    desc:string,
    areaCode:number,
  } = {
    role:'',
    desc:'',
    areaCode:0,
    payPerHour:0
  }
  messageList:any[]=[];

  constructor(private userService:UserService, private modalService:NgbModal, private generalService:GeneralService){};

  ngOnInit() {
    this.userService.getUserSubject().subscribe(e => {
      this.user = e;
    });
    this.generalService.getAreas().subscribe((data:any)=>{
      this.areas = data;
    });
    this.generalService.getAllMessages(0).subscribe((data:any)=>{
      this.messageList = data;
    });
  }

  openModal(content: TemplateRef<any>) {
    this.modalService.open(content, {centered: true}).result.then((result) => {
      this.post.areaCode = 0;
      this.post.payPerHour = 0;
      this.post.desc = '';
      this.post.role = '';
    }).catch((res) => {});
  }

  createPost(){
    this.generalService.createPost(this.post).subscribe((data:any)=>{
      this.messageList = data;
    });
    this.post.areaCode = 0;
    this.post.payPerHour = 0;
    this.post.desc = '';
    this.post.role = '';
  }

  getProfile(url : string) {
    return this.userService.getProfileUrl(url);
  }

  requestForMessage(){
    this.generalService.getAllMessages(this.areaId??0).subscribe((data:any)=>{
      this.messageList = data;
    });
  }

}
