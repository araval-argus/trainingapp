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

  user: LoggedInUser;

  // flags
  dataLoadingFlag: boolean = true;

  // utilities
  timeStamp = new Date();
  userProfileImageLink: string = "/assets/images/placeholder.jpg";
  hostUrl: string = environment.hostUrl;

  constructor(
    private authService: AuthService,
    private userService: UserService
  ) { 
    this.fetchProfileUrl();
  }

  ngOnInit(): void {
    this.fetchUserDetails();
  }

  private fetchUserDetails() {
    this.userService.getCurrentUserDetails().subscribe((result) => {
      this.user = result;
      console.log(result);
      this.dataLoadingFlag = false;
    },
    (err) => {
      console.log(err);
      this.dataLoadingFlag = false;
    });
  }

  public getLinkPicture() {
    if(this.timeStamp) {
       return this.userProfileImageLink + '?' + this.timeStamp;
    }
    return this.userProfileImageLink;
  }

  fetchProfileUrl() {
    this.userService.getUserProfileUrl().subscribe(
      (res) => {
        if (res.profileUrl == "") {
          return;
        }

        this.userProfileImageLink = this.hostUrl + "/" + res.profileUrl;
        this.updateProfileImage();
      },
      (err) => {
        console.log(err);
      }
    )
  }

  public updateProfileImage() {
    
    this.timeStamp = new Date();
    // this.userProfileImageLink + '?' + this.timeStamp;
}


}
