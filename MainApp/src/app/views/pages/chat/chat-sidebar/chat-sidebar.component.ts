import { Component, OnInit} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbDropdown } from '@ng-bootstrap/ng-bootstrap';
import { ColleagueModel } from 'src/app/core/models/colleague-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { MessageDisplayModel } from 'src/app/core/models/message-display-model';
import { RecentChatModel } from 'src/app/core/models/recent-chat-model';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';
import { SignalRService } from 'src/app/core/service/signalr-service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-chat-sidebar',
  templateUrl: './chat-sidebar.component.html',
  styleUrls: ['./chat-sidebar.component.scss'],
})

export class ChatSidebarComponent implements OnInit {

  loggedInUser : LoggedInUser;
  imageSource : string;
  colleagues : ColleagueModel [] = [] ;
  recentChatList : RecentChatModel[] =[];
  ImageStartUrl = environment.ImageUrl;
  showDropDown : boolean = false;
  statusList : {id : number, status : string}[] = [];

  constructor(private authService : AuthService ,private router : Router , private accountService : AccountService,
    private route : ActivatedRoute , private chatService : ChatService  , private signalRService : SignalRService) { }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    //console.log(this.loggedInUser);
    this.imageSource = environment.ImageUrl + this.loggedInUser.imagePath;
    this.fetchRecentChat();

    this.accountService.getAllStatus().subscribe((data:any)=>{
      this.statusList = data;
    });

    this.accountService.getUserStatus(this.loggedInUser.userName).subscribe((data:{id : number, status : string})=>{
      this.loggedInUser.status=data.status;
    });

    this.chatService.DidAMessage.subscribe(()=>{
      this.fetchRecentChat();
    });

    this.signalRService.hubConnection.on('recieveMessage',(msg:MessageDisplayModel)=>{
       this.chatService.loadRecentChat().subscribe((data:RecentChatModel[])=>{
        this.recentChatList = data;
       })
    });

    this.signalRService.hubConnection.on('msgSeen',(msgFrom:string)=>{
      this.removeMarkCount(msgFrom);
    });

    this.signalRService.hubConnection.on('userStatusChanged',(userName:string,statusString:string)=>{
      const index = this.recentChatList.findIndex(u => u.userName === userName);
      if (index !== -1) {
        this.recentChatList[index].status = statusString;
      }
    });
  }

  fetchRecentChat(){
    this.chatService.loadRecentChat().subscribe((data:RecentChatModel[])=>{
      this.recentChatList = data;
      console.log(this.recentChatList);
    });
  }

  onColleagueSelected(selUser : ColleagueModel){
    this.Outside();
    this.recentChatList.forEach((recent)=>{
      if(recent.userName === selUser.userName){
      recent.seen = 0;
      }
    })
   }

   onSearch(event:any){
    this.showDropDown = true;
    if(event.target.value.length>2){
      this.chatService.search(event.target.value)
        .subscribe((data: ColleagueModel[]) => {
         //console.log(data);
         this.colleagues = data;
       });
      }
    }

   onEditProfile(){
    this.router.navigate(['../update-profile'],{relativeTo:this.route});
   }

   onViewProfile(){
    this.router.navigate(['../view-profile'],{relativeTo:this.route});
   }

   markAllAsRead(){
    this.chatService.markAsRead('All').subscribe();
    this.recentChatList.forEach((recent)=>{
      recent.seen = 0;
    })
   }

   markAsRead(e:Event , username:string){
    e.preventDefault();
    e.stopPropagation();
    this.chatService.markAsRead(username).subscribe();
    this.removeMarkCount(username);
   }

   removeMarkCount(username : string){
    var recent = this.recentChatList.find(recent=>recent.userName===username);
    recent.seen = 0;
   }

   Outside(){
    this.showDropDown = false;
   }

   recentChatDate(date: Date): string {
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

  statusChange(i:number,status:string){
    this.loggedInUser.status = status;
    this.accountService.changeStatus(i,this.loggedInUser.userName).subscribe();
  }

  ngAfterViewInit(): void {
    // Show chat-content when clicking on chat-item for tablet and mobile devices
    document.querySelectorAll('.chat-list .chat-item').forEach(item => {
      item.addEventListener('click', event => {
        document.querySelector('.chat-content').classList.toggle('show');
      })
    });
  }

  // back to chat-list for tablet and mobile devices
  backToChatList() {
    document.querySelector('.chat-content').classList.toggle('show');
  }

  save() {
    console.log('passs');
  }

  over(drop:NgbDropdown){
    drop.open()
  }
  out(drop:NgbDropdown){
    drop.close()
  }

}


