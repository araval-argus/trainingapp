import { Component, OnInit } from '@angular/core';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { AuthService } from 'src/app/core/service/auth-service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-viewprofile',
  templateUrl: './viewprofile.component.html',
  styleUrls: ['./viewprofile.component.scss']
})
export class ViewprofileComponent implements OnInit {

  loggedInUser:LoggedInUser;
  ImageSource : string;
  constructor(private authService : AuthService) { }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.ImageSource = environment.ImageUrl + this.loggedInUser.ImagePath;
    }
}


