import { Component, EventEmitter, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelper } from 'src/app/core/helper/jwt-helper';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import Swal from 'sweetalert2'
import { environment } from 'src/environments/environment';

@Component({
  selector: "app-edit-form",
  templateUrl: "./edit-form.component.html",
  styleUrls: ["./edit-form.component.scss"],
})
export class EditFormComponent implements OnInit {
  constructor(
    private authService: AuthService,
    private accountService: AccountService,
    private router: Router,
    private jwtHelper: JwtHelper
  ) {}

  loggedInUserChanged = new EventEmitter();

  loggedInUser: LoggedInUser;
  usernameExists: boolean = false;
  imgFile: File;
  timeOutId;

  //for displaying image
  imgUrl: any;

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.loggedInUser.userName = this.loggedInUser.sub;
    this.imgUrl = this.loggedInUser.imageUrl;
  }

  onUpdate(e: Event) {
    e.preventDefault();
    console.log("in onUpdate method ", this.loggedInUser);

    const formData = new FormData();

    formData.append("imageFile", this.imgFile);
    formData.append("userName", this.loggedInUser.userName);
    formData.append("firstName", this.loggedInUser.firstName);
    formData.append("lastName", this.loggedInUser.lastName);
    formData.append("email", this.loggedInUser.email);

    this.accountService.update(formData).subscribe(
      (data: any) => {
        console.log(data);
        this.authService.login(data.token, () => {
          Swal.fire({
            title: "Success!",
            text: "Details has been updated",
            icon: "success",
            timer: 2000,
            timerProgressBar: true,
          });
          setTimeout(() => {
            console.log("navigating to dashboard after updating....");
            this.loggedInUserChanged.emit();
            this.router.navigate(["/"]);
          }, 3000);
        });
      },
      (err) => {
        console.log(err);
      }
    );
  }


  onUserNameValueChange(event: Event) {
    const value = (event.target as HTMLInputElement).value;


    console.log("at the begining : ", this.timeOutId)
    if (this.timeOutId) {
      clearTimeout(this.timeOutId);
    }

    this.timeOutId =  setTimeout(() => {
      //compare the username stored in token with new edited username
      const token = this.authService.getUserToken();
      const decodedToken = this.jwtHelper.decodeToken(token);

      //console.log("timeout in username change")

      //dont send a request if both are equal
      if (value !== decodedToken.sub) {
        this.accountService.checkUsername(value).subscribe((data) => {
          console.log(data.usernameExists);
          this.usernameExists = data.usernameExists;

        });
      }
    }, 1000);

    console.log("at the end : "+ this.timeOutId)
  }

  onFileChange(event: Event) {
    this.imgFile = (event.target as HTMLInputElement).files[0];
    //this.imgUrl = this.imgFile.;
    console.log(this.imgFile);
    // console.log(img);

    var reader = new FileReader();
    reader.onload = (e) => {
      this.imgUrl = e.target.result;
    };
    if(this.imgFile){
      console.log("inside if")
      reader.readAsDataURL(this.imgFile);
    }
  }

  //  const formData = new FormData();
  //     formData.append('imageData', );
  //     console.log(formData.get('imageData'));
  //     this.accountService.dummyRequest(formData).subscribe(data =>{
  //       console.log(data);
  //     },error => {
  //       console.log(error);
  //     })
}
