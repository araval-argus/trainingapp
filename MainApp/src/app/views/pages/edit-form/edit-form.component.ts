import { Component,  EventEmitter, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { JwtHelper } from 'src/app/core/helper/jwt-helper';
import { LoggedInUserModel } from 'src/app/core/models/loggedin-user';
import { PasswordModel } from 'src/app/core/models/password-model';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import Swal from 'sweetalert2'

@Component({
  selector: "app-edit-form",
  templateUrl: "./edit-form.component.html",
  styleUrls: ["./edit-form.component.scss"],
})
export class EditFormComponent implements OnInit {

  LoggedInUserModelChanged = new EventEmitter();

  loggedInUser: LoggedInUserModel;
  usernameExists: boolean = false;
  imgFile: File;
  timeOutIdForUsername;
  timeOutIdForPassword;
  passwordModel?: PasswordModel;
  passwordMatched: boolean = false;
  //for displaying image
  imgUrl: any;


  constructor(
      private authService: AuthService,
      private accountService: AccountService,
      private router: Router,
      private jwtHelper: JwtHelper,
      private modalService: NgbModal
    ) {}

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.loggedInUser.userName = this.loggedInUser.sub;
    this.imgUrl = this.loggedInUser.imageUrl;
    console.log(this.loggedInUser);
    this.passwordModel = new PasswordModel();
    this.passwordModel.username = this.loggedInUser.userName;
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
            this.LoggedInUserModelChanged.emit();
            this.router.navigate(["/"]);
          }, 800);
        });
      },
      (err) => {
        console.log(err);
      }
    );
  }


  onUserNameValueChange(event: Event) {
    const value = (event.target as HTMLInputElement).value;


    if (this.timeOutIdForUsername) {
      clearTimeout(this.timeOutIdForUsername);
    }

    this.timeOutIdForUsername =  setTimeout(() => {
      //compare the username stored in token with new edited username
      const token = this.authService.getUserToken();
      const decodedToken = this.jwtHelper.decodeToken(token);

      console.log("timeout in username change")

      //dont send a request if both are equal
      if (value !== decodedToken.sub) {
        this.accountService.checkUsername(value).subscribe((data) => {
          console.log(data.usernameExists);
          this.usernameExists = data.usernameExists;

        });
      }
    }, 1000);

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

  openVerticalCenteredModal(content) {
    this.modalService.open(content, {centered: true}).result.then((result) => {
      console.log("Modal closed" + result);
      this.passwordModel.oldPassword = "";
      this.passwordModel.newPassword = "";
    }).catch((res) => {});
  }

  checkPassword(currPassword: string){
    if(this.timeOutIdForPassword){
      clearTimeout(this.timeOutIdForPassword)
    }

    this.timeOutIdForPassword = setTimeout( () => {
      this.accountService.checkCurrPassword(this.loggedInUser.userName, currPassword).subscribe((data: any) =>{
        this.passwordMatched = data.passwordMatched;
      });
    }, 400);
  }

  onPasswordChange(){
    console.log("onPasswordChange");
    this.accountService.changePassword(this.passwordModel).subscribe( data => {
      console.log(data)
    }, err => {
      console.log(err)
      Swal.fire({
        icon: 'error',
        title: 'Password Incorrect',
        text: 'You have entered wrong current password',
        footer: '<a href="">Why do I have this issue?</a>'
      })
    });
  }
}
