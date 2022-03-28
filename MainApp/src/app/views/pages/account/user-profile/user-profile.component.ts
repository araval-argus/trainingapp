import { Component, OnInit } from '@angular/core';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { AuthService } from 'src/app/core/service/auth-service';
import { UserService } from 'src/app/core/service/user.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent implements OnInit {

  userObj: LoggedInUser;

  // flags
  dataLoadingFlag: boolean = true;

  // utilities
  timeStamp = new Date();
  userProfileImageLink: string = "/assets/images/placeholder.jpg";
  hostUrl: string = environment.hostUrl;

  constructor(private authService: AuthService, private userService: UserService) { }

  async ngOnInit(): Promise<void> {
    if (!this.userService.currentUserObject) {
      console.log("This user object is undefined");
      await this.userService.setCurrentUserSubject();
    }
 
    this.userService.currentUserObject.subscribe(userObj => {  this.userObj = userObj; } )

    this.dataLoadingFlag = false;
  
  }



  public getImageLink() {
    return this.userObj.profileUrl ? this.userObj.profileUrl + '?' + this.timeStamp : this.userProfileImageLink;
    if(this.userObj.profileUrl) {
       return this.userObj.profileUrl + '?' + this.timeStamp;
    }
    return this.userProfileImageLink;
  }


  public updateProfileImage() { this.timeStamp = new Date(); }


}
