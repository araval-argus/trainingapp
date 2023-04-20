import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { LoginModel } from 'src/app/core/models/login-model';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import { SignalRService } from 'src/app/core/service/signalR-service';
import Swal from 'sweetalert2'

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  returnUrl: any;
  loginModel: LoginModel
  constructor(private router: Router,
    private route: ActivatedRoute,
    private accountService: AccountService,
    private authService: AuthService,
    private signalRService: SignalRService) { }

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
   // console.log(this.loginModel);

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
        }, (500));

        this.router.navigate([this.returnUrl]);

      });
      this.signalRService.makeConnection();
    }, (err) => {
      Swal.fire({
        title: 'Error!',
        text: err.error.message,
        icon: 'error',
      });
    });

  }

}
