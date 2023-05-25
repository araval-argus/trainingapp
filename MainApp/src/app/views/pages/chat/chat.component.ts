import { Component, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { LoggedInUserModel } from 'src/app/core/models/loggedin-user';
import { MessageModel } from 'src/app/core/models/message-model';
import { UserModel } from 'src/app/core/models/UserModel';
import { AuthService } from 'src/app/core/service/auth-service';
import { ChatService } from 'src/app/core/service/chat-service';
import { SignalRService } from 'src/app/core/service/signalR-service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements OnInit, AfterViewInit, OnDestroy {

  defaultNavActiveId = 1;
  subscriptions : Subscription[] = [];
  selectedFriend: UserModel;

  friends: UserModel[] = [];

  loggedInUser: LoggedInUserModel;

  constructor(private chatService: ChatService, private authService: AuthService,private signalRService: SignalRService) {
  }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.subscribeToFriendSelected();
    this.subscribeToFetchFriends();
    this.addMessageSubscription();
  }

  ngAfterViewInit(): void {
    // Show chat-content when clicking on chat-item for tablet and mobile devices
    document.querySelectorAll('.chat-list .chat-item').forEach(item => {
      item.addEventListener('click', event => {
        document.querySelector('.chat-content').classList.toggle('show');
      })
    });

  }



  addMessageSubscription(){
    let sub = this.signalRService.messageAdded.subscribe((message: MessageModel) => {

      //sender side
      if(message.senderUserName === this.loggedInUser.sub){

        const friend = this.friends.find( friend => friend.userName === message.recieverUserName );

        //case 1: Reciever not present in the list  then add it to the top of the list without incrementing the unread count: (Tested Successfully)
        if(!friend){
          //console.log("friend not found")
          this.chatService
          .fetchUser(message.recieverUserName)
          .subscribe((friend: UserModel) => {
            friend.imageUrl = environment.apiUrl + "/../Images/Users/" + friend.imageUrl;
            friend.lastMessage = message.message;
            friend.lastMessageTimeStamp = new Date(friend.lastMessageTimeStamp + 'Z');
            this.friends.splice(0, 0, friend);
          });
        }

        //case 2: Reciever is present in the list   then move it to the top of the list with setting the last message and its timestamp (Tested Successfully)
        else{
          this.selectedFriend.lastMessage = message.message;
          this.selectedFriend.lastMessageTimeStamp = new Date(message.createdAt);
          this.friends.splice(this.friends.indexOf(friend), 1);
          this.friends.splice(0,0,friend);
        }


      }

      //reciever side
      else{
        //find sender from the list
        const friend = this.friends.find( friend => friend.userName === message.senderUserName);

        //case 1: Sender not present in the list   then add sender to the top of the list with unread count 1 (Tested Successfully)
        if (!friend) {
          this.chatService
            .fetchUser(message.senderUserName)
            .subscribe((friend: UserModel) => {
              friend.imageUrl =
                environment.apiUrl + "/../Images/Users/" + friend.imageUrl;
              friend.lastMessage = message.message;
              friend.lastMessageTimeStamp = new Date(
                friend.lastMessageTimeStamp + "Z"
              );
              friend.unreadMessageCount = 1;
              this.friends.splice(0, 0, friend);
            });
        }

        //case 2: Sender present in the list:
        else {
          this.friends.splice(this.friends.indexOf(friend), 1);
          friend.lastMessage = message.message;
          friend.lastMessageTimeStamp = new Date(message.createdAt);

          //case 2.1: Selected Friend is Sender   then move sender to the top of the list with unread count as 0 and
          //           also send the request to mark this message as seen (Tested Successfully)
          if (
            this.selectedFriend &&
            friend.userName === this.selectedFriend.userName
          ) {
            friend.unreadMessageCount = null;
            this.signalRService.markMsgAsSeen(message);
          }

          //case 2.2: Selected Friend is not Sender  then move sender to the top of the list with incremented unread count (Tested Successfully)
          else {
            friend.unreadMessageCount += 1;
          }

          this.friends.splice(0, 0, friend);
        }

      }

    });

    this.subscriptions.push(sub);
  }

  subscribeToFriendSelected(){
    let sub = this.chatService.friendSelected.subscribe(profile => {
      this.selectedFriend = profile;
      this.signalRService.notificationRemoved.next(profile.userName);
    })
    this.subscriptions.push(sub);
  }

  subscribeToFetchFriends(){
    let sub = this.chatService.fetchAllInteractedUsers(this.authService.getLoggedInUserInfo().sub).subscribe((data: UserModel[]) => {
      this.friends = data;
      this.friends.forEach((friend)=>{
        friend.imageUrl = environment.apiUrl + "/../Images/Users/" + friend.imageUrl;
        friend.lastMessageTimeStamp = new Date(friend.lastMessageTimeStamp + 'Z');
      })
    }, err => {
      console.log(err);
    })

    this.subscriptions.push(sub);
  }

  ngOnDestroy(){
    this.subscriptions.forEach( subscription => subscription.unsubscribe());
  }
}
