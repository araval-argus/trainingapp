import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { FriendProfileModel } from 'src/app/core/models/friend-profile-model';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, AfterViewInit, OnDestroy {

  defaultNavActiveId = 1;

  selectedFriend: FriendProfileModel;

  friends: FriendProfileModel[] = [];



  constructor(private chatService: ChatService, private authService: AuthService) { }

  ngOnInit(): void {
    this.chatService.friendSelected.subscribe(profile => {
      this.selectedFriend = profile;
    })
    this.chatService.fetchAll(this.authService.getLoggedInUserInfo().sub).subscribe((data:any) => {
      this.friends = data.data;
     // console.log("all users fetched" , this.friends)
      this.friends.forEach((friend, index)=>{
        this.friends[index].imageUrl = environment.apiUrl + "/../Images/Users/" + friend.imageUrl;
      })
    })
  }

  ngAfterViewInit(): void {

    // Show chat-content when clicking on chat-item for tablet and mobile devices
    document.querySelectorAll('.chat-list .chat-item').forEach(item => {
      item.addEventListener('click', event => {
        document.querySelector('.chat-content').classList.toggle('show');
      })
    });

  }



  save() {
    console.log('passs');
  }

  ngOnDestroy(){
   // this.chatService.friendSelected.unsubscribe();
  }

}
