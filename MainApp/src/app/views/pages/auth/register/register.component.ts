import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgModel } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { DesignationModel } from 'src/app/core/models/designation-model';
import { RegistrationModel } from 'src/app/core/models/registration-model';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import { SignalRService } from 'src/app/core/service/signalR-service';
import Swal from 'sweetalert2'

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit, OnDestroy {
  regModel: RegistrationModel;
  disableRegButtton: boolean = true;
  designations: DesignationModel[];
  timeoutIdForUserName;
  userNameExists: boolean;
  subscriptions: Subscription[] = [];

  constructor(private router: Router,
    private accountService: AccountService,
    private authService: AuthService,
    private signalRService: SignalRService) { }

  ngOnInit(): void {
    this.regModel = {
      firstName: '',
      lastName: '',
      userName: '',
      email: '',
      password: '',
      designationID: 1
    }

    const sub = this.accountService.fetchDesignations().subscribe((data:any) => {
      this.designations = data.designations;
    })

    this.subscriptions.push(sub);

  }

  onRegister(e) {
    e.preventDefault();
    if(this.userNameExists){
      Swal.fire({
        icon: 'error',
        title: 'Username Already Exists',
        text: 'This username already exists please enter different username',
        timer: 1500,
        timerProgressBar: true
      })
    }
    else{
    const sub = this.accountService.register(this.regModel)
      .subscribe((data: any) => {
        this.signalRService.makeConnection();
        this.authService.login(data.token, ()=>{
          Swal.fire({
            title: 'Success!',
            text: 'User has been registered.',
            icon: 'success',
            timer: 2000,
           timerProgressBar: true,
          });
          setTimeout(() => {
            this.router.navigate(["/"]);

          }, (1000));
        })

      }, (err) => {
        this.disableRegButtton = false;
        Swal.fire({
          title: 'Error!',
          text: err.error.message,
          icon: 'error',
        });
      });
      this.subscriptions.push(sub);
    }

  }

  onUserNameChanged(event: Event){
    this.userNameExists = false;
    const value = (event.target as HTMLInputElement).value;
    if(this.timeoutIdForUserName){
      clearTimeout(this.timeoutIdForUserName);
    }

    setTimeout( () => {
      this.accountService.checkUsername(value).subscribe( data => {
        this.userNameExists = data.usernameExists;
      })
    },800)
  }


  ngOnDestroy(){
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

}
