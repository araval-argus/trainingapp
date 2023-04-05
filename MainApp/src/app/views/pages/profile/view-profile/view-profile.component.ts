import { Component, OnInit } from '@angular/core';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';

@Component({
  selector: 'app-view-profile',
  templateUrl: './view-profile.component.html',
  styleUrls: ['./view-profile.component.scss']
})
export class ViewProfileComponent implements OnInit {
  loggedInUser: LoggedInUser
  imageURL: string = localStorage.getItem('imagePath');
  constructor(private authService: AuthService, private accountService: AccountService) { }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.accountService.updateDetails.subscribe(() => {
      this.imageURL = localStorage.getItem('imagePath');
    })
  }

}
