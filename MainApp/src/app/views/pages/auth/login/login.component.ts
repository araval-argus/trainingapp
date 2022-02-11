import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { LoginModel } from 'src/app/core/models/login-model';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import Swal from 'sweetalert2'

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  returnUrl: any;
  loginModel: LoginModel

  // utils
  isFormValid: boolean = true;
  isEmailValid: boolean = true;
  isPasswordValid: boolean = true;
  isPasswordValidLength: boolean = true;

  constructor(private router: Router,
    private route: ActivatedRoute,
    private accountService: AccountService,
    private authService: AuthService) { }

  ngOnInit(): void {
    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    this.loginModel = {
      emailAddress: '',
      password: '',
      userName: ''
    }
  }

  onLoggedin(e) {
    e.preventDefault();
    console.log(this.loginModel);

    this.checkValidForm()

    // validation of form
    if (!this.isFormValid) {
      console.log("Validation failed!")
      return;
    }

    // Implementation of API.
    this.accountService.login(this.loginModel).subscribe((result: any) => {
      this.authService.login(result.token, () => {
        Swal.fire({
          title: 'Success!',
          text: 'User loggedin successfully.',
          icon: 'success',
          timer: 2000,
         timerProgressBar: true,
        });
        setTimeout(() => {
          this.router.navigate(["/"]);
        }, (3000));
        this.router.navigate([this.returnUrl]);
      });
    }, (err) => {
      Swal.fire({
        title: 'Error!',
        text: err.error.message,
        icon: 'error',
      });
    });

  }

  checkValidForm(): boolean {
    if (this.loginModel.emailAddress.length == 0 || this.loginModel.password.length == 0) {
      this.isFormValid = false;
      return false;
    }
    return true;
  }

  checkEmail() {
    this.isEmailValid = true;
    if (this.loginModel.emailAddress.length == 0) {
      this.isEmailValid = false;
    }
  }

  checkPassword() {
    this.isPasswordValid = true;
    this.isPasswordValidLength = true;

    if( this.loginModel.password.length == 0) {
      this.isPasswordValid = false;
      return;
    }

    if (this.loginModel.password.length < 8) {
      this.isPasswordValidLength = false;
      return;
    }
  }

}
