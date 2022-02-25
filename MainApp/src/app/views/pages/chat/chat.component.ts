import { AfterViewInit, Component, OnInit } from '@angular/core';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { UserService } from 'src/app/core/service/user.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: []
})
export class ChatComponent implements OnInit {

  userObj: LoggedInUser;

  // flags
  dataLoadingFlag: boolean = true;

  constructor(private userService: UserService) { }

  async ngOnInit(): Promise<void> {
    if (!this.userService.currentUserObject) {
      await this.userService.setCurrentUserSubject();
    }
 
    this.userService.currentUserObject.subscribe(userObj => {  this.userObj = userObj; } )

    this.dataLoadingFlag = false;

  }

    
  
  
}
