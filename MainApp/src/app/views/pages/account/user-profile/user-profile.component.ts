import { Component, OnInit } from '@angular/core';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { AuthService } from 'src/app/core/service/auth-service';
import { UserService } from 'src/app/core/service/user.service';

@Component({
  selector: 'app-user-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent implements OnInit {

  user: LoggedInUser;
  allUsers: LoggedInUser[];

  // flags
  dataLoadingFlag: boolean = true;


  constructor(
    private authService: AuthService,
    private userService: UserService
  ) { }

  ngOnInit(): void {
    this.fetchUserDetails();
    console.table(this.user)
    console.log("-----------------");
    this.getAllUsers();
    
  }

  private fetchUserDetails() {
    this.user = this.authService.getLoggedInUserInfo();
  }

  private getAllUsers() {
    this.userService.getUsers().subscribe(data => {
      this.allUsers = data;
      console.table(data);
    })

  }

}
