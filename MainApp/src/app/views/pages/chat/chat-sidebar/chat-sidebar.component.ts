import { Component, OnInit} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ColleagueModel } from 'src/app/core/models/colleague-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { MessageDisplayModel } from 'src/app/core/models/message-display-model';
import { RecentChatModel } from 'src/app/core/models/recent-chat-model';
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

  defaultNavActiveId = 1;
  loggedInUser : LoggedInUser;
  imageSource : string;
  colleagues : ColleagueModel [] = [] ;
  recentChatList : RecentChatModel[] =[];
  ImageStartUrl = environment.ImageUrl;
  showDropDown : boolean = false;

  constructor(private authService : AuthService ,private router : Router ,
    private route : ActivatedRoute , private chatService : ChatService  , private signalRService : SignalRService) { }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    //console.log(this.loggedInUser);
    this.imageSource = environment.ImageUrl + this.loggedInUser.imagePath;
    this.fetchRecentChat();

    this.chatService.DidAMessage.subscribe(()=>{
      this.fetchRecentChat();
    });

    this.signalRService.hubConnection.on('recieveMessage',(msg:MessageDisplayModel)=>{
       this.chatService.loadRecentChat().subscribe((data:RecentChatModel[])=>{
        this.recentChatList = data;
       })
    })
  }

  fetchRecentChat(){
    this.chatService.loadRecentChat().subscribe((data:RecentChatModel[])=>{
      this.recentChatList = data;
    })
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

}


