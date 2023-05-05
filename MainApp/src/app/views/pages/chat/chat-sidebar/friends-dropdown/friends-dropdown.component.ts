import { Component, OnDestroy, OnInit } from '@angular/core';
import { UserModel } from 'src/app/core/models/UserModel';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-friends-dropdown',
  templateUrl: './friends-dropdown.component.html',
  styleUrls: ['./friends-dropdown.component.scss']
})
export class FriendsDropdownComponent implements OnInit, OnDestroy {

  timeOutId;
  apiUrl: string = environment.apiUrl;
  friendsProfiles: UserModel[];

  constructor(private chatService: ChatService,
    private authService: AuthService) { }

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
            this.friendsProfiles = data.message as UserModel[]
            for(let friend of this.friendsProfiles){
              friend.imageUrl = this.apiUrl + "/../Images/Users/" + friend.imageUrl
            }
            //console.log("friends profile array",this.friendsProfiles);
          },
          error => {
            console.log(error)
          })
    }, 300);

  }

  friendSelected(friend :UserModel){
    this.chatService.friendSelected.next(friend);
    //console.log("inside dropdown component :- ", friend)
    this.chatService.fetchMessages(this.authService.getLoggedInUserInfo().sub, friend.userName).subscribe((data)=>{
      this.chatService.messagesRecieved.next(data);
    });
    this.emptyList();
  }

  emptyList(){
    this.friendsProfiles = [];
  }

  ngOnDestroy(): void {
    clearTimeout(this.timeOutId);
  }

}
