import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { FriendProfile } from 'src/app/core/models/friend-profile-model';
import { ChatService } from 'src/app/core/service/chat-service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, AfterViewInit, OnDestroy {

  defaultNavActiveId = 1;

  selectedFriend: FriendProfile;

  constructor(private chatService: ChatService) { }

  ngOnInit(): void {
    //console.log("inside chat component")
    this.chatService.friendSelected.subscribe(profile => {
      //console.log("frined selected msg from chat component")
      this.selectedFriend = profile;
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
