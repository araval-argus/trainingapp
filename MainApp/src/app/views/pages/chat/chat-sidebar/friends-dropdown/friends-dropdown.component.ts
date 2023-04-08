import { Component, OnInit } from '@angular/core';
import { FriendProfile } from 'src/app/core/models/friend-profile-model';
import { ChatService } from 'src/app/core/service/chat-service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-friends-dropdown',
  templateUrl: './friends-dropdown.component.html',
  styleUrls: ['./friends-dropdown.component.scss']
})
export class FriendsDropdownComponent implements OnInit {
  timeOutId;
  apiUrl: string = environment.apiUrl;
  constructor(private chatService: ChatService) { }
  friendsProfiles: FriendProfile[];
  ngOnInit(): void {
  }

  onInputChange(event: Event){

    let searchTerm = (event.target as HTMLInputElement).value;
    if(this.timeOutId){

      clearTimeout(this.timeOutId);
    }

    this.timeOutId = setTimeout( () => {
      this.chatService.fetchFriendsName(searchTerm).subscribe( (data: any) => {
            //console.log(data)
            this.friendsProfiles = data.message as FriendProfile[]
            for(let friend of this.friendsProfiles){
              friend.imageUrl = this.apiUrl + "/../Images/Users/" + friend.imageUrl
            }
            //console.log("friends profile array",this.friendsProfiles);
          },
          error => {
            console.log(error)
          })
    }, 1000);

  }

  friendSelected(){
    this.emptyList();
  }
  emptyList(){
    this.friendsProfiles = [];
  }
}
