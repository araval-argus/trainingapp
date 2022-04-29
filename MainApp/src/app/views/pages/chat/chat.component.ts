import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { UserService } from 'src/app/core/service/user.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: []
})
export class ChatComponent implements OnInit {

  userObj: LoggedInUser;

  allUsers: LoggedInUser[] = [];

  userToChat: string;

  // flags
  dataLoadingFlag: boolean = true;

  // flags to pass
  allUsersLoadingFlag: boolean = true;

  constructor(private userService: UserService, private activatedRoute: ActivatedRoute) { }

  async ngOnInit(): Promise<void> {
    if (!this.userService.currentUserObject) {
      await this.userService.setCurrentUserSubject();
    }
 
    this.userService.currentUserObject.subscribe(userObj => {  this.userObj = userObj; } )

    this.dataLoadingFlag = false;

    this.fetchUsers();

    this.userToChat =  this.activatedRoute.snapshot.paramMap.get("user");
    
  }

  fetchUsers() {
    this.userService.getUsers().subscribe(
      (res) => {
        this.allUsers = res;
        this.allUsersLoadingFlag = false;
      },
      (err) => {
        console.log(err);
        alert("Some error in fetching all users!")
      }
    )
  }
  
  changeUser() {
    console.log("User changed");
    this.userToChat =  this.activatedRoute.snapshot.paramMap.get("user");
  }
  
}
