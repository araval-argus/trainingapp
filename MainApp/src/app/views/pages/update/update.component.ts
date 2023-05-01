import { Component, OnInit  } from '@angular/core';

import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import { environment } from 'src/environments/environment';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-update',
  templateUrl: './update.component.html',
  styleUrls: ['./update.component.scss']
})

export class UpdateComponent implements OnInit {

  loggedInUser : LoggedInUser;
  imageFile : File;
  imageUrl : string;

  constructor(private authService : AuthService, private accountService : AccountService) { }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.imageUrl = environment.ImageUrl + this.loggedInUser.imagePath;
  }

  onFileSelected(event) {
    if (event.target.files.length > 0) {
      this.imageFile = (event.target as HTMLInputElement).files[0];
     // console.log(this.imageFile);
    }
  }

  onSubmit(){
    const formdata = new FormData();
    formdata.append('username',this.loggedInUser.userName);
    formdata.append('firstName' ,this.loggedInUser.firstName);
    formdata.append('lastName' ,this.loggedInUser.lastName);
    formdata.append('email' ,this.loggedInUser.email);
    formdata.append('designation',this.loggedInUser.designation);
    formdata.append('profileImage',this.imageFile);
    this.accountService.update(formdata)
      .subscribe((data:any) => {
        this.authService.login(data.token, () => {
          Swal.fire({
            title: 'Success!',
            text: 'Profile Updated SuccessFully',
            icon: 'success',
            timer: 2000,
           timerProgressBar: true,
          });
        });
        this.authService.UserProfileChanged.next();
      }, (err) => {
        Swal.fire({
          title: 'Error!',
          text: err.error.message,
          icon: 'error',
        });
      })
  }
}
