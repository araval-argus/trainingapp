import { Component, OnInit} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ColleagueModel } from 'src/app/core/models/colleague-model';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { RecentChatModel } from 'src/app/core/models/recent-chat-model';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-chat-sidebar',
  templateUrl: './chat-sidebar.component.html',
  styleUrls: ['./chat-sidebar.component.scss'],
})
export class ChatSidebarComponent implements OnInit {

  defaultNavActiveId = 1;
  loggedInUser : LoggedInUser;
  ImageSource : string;
  colleagues : ColleagueModel [] = [] ;
  RecentChatList : RecentChatModel[] =[];
  ImageStartUrl = environment.ImageUrl;

  constructor(private authService : AuthService ,private router : Router ,
    private route : ActivatedRoute , private chatService : ChatService ) { }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.ImageSource = environment.ImageUrl + this.loggedInUser.ImagePath;
    this.FetchRecentChat();
    this.chatService.DidAMessage.subscribe(()=>{
      this.FetchRecentChat();
    });
  }

  FetchRecentChat(){
    this.chatService.LoadRecentChat().subscribe((data:RecentChatModel[])=>{
      this.RecentChatList = data;
    })
  }

  onColleagueSelected(selUser : ColleagueModel){
    console.log('User Selected');
   }

   onSearch(event:any){
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
