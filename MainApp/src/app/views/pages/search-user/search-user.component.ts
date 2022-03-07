import { Component, OnInit } from '@angular/core';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { UserService } from 'src/app/core/service/user.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-search-user',
  templateUrl: './search-user.component.html',
  styleUrls: ['./search-user.component.scss']
})
export class SearchUserComponent implements OnInit {

  userToSearch: string = "";

  searchResult: LoggedInUser[];
  searchLoadingFlag: boolean = false;

  constructor(private userService: UserService) { }

  ngOnInit(): void {
    this.searchResult = [];
  }

  searchUser(user:string) {

    if(user.length < 2) return;
    this.userToSearch = user;
    this.searchLoadingFlag = true;
    this.userService.searchUser(user).subscribe(
      (res) => {
        console.log(res);
        this.searchResult = res;
        this.searchLoadingFlag = false;
      },
      (err) => {
        console.log(err);
        alert("Error in searching the user")
      }
    )
  }

  getProfileUrl(url:string) {
    if (url == null) {
      return "/assets/images/placeholder.jpg";
    }
    else {
      return environment.hostUrl + "/" + url;
    }
  }

}
