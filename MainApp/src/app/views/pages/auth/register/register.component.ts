import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { RegistrationModel } from 'src/app/core/models/registration-model';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import Swal from 'sweetalert2'

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  regModel: RegistrationModel;
  disableRegButtton: boolean = false;

  @ViewChild('firstNameInput') firstname;
  constructor(private router: Router,
    private accountService: AccountService,
    private authService: AuthService) { }

  ngOnInit(): void {
    this.regModel = {
      firstName: '',
      lastName: '',
      userName: '',
      email: '',
      password: '',
    }
  }

  onRegister(e) {
    e.preventDefault();
    console.log(this.regModel);
    console.log(this.firstname);
    // this.disableRegButtton = true;
    // this.accountService.register(this.regModel)
    //   .subscribe((data: any) => {
    //     this.authService.login(data.token, ()=>{
    //       Swal.fire({
    //         title: 'Success!',
    //         text: 'User has been registered.',
    //         icon: 'success',
    //         timer: 2000,
    //        timerProgressBar: true,
    //       });
    //       setTimeout(() => {
    //         this.router.navigate(["/"]);
    //         this.disableRegButtton = false;

    //       }, (3000));
    //     })

    //   }, (err) => {
    //     this.disableRegButtton = false;
    //     Swal.fire({
    //       title: 'Error!',
    //       text: err.error.message,
    //       icon: 'error',
    //     });
    //   });

  }

  private validate = (regModel: RegistrationModel) => {
    if(regModel.firstName === '' || regModel.firstName === undefined){return}
    if(regModel.lastName === '' || regModel.lastName === undefined){return}
    if(regModel.email === '' || regModel.email === undefined){return}
    if(regModel.userName === '' || regModel.userName === undefined){return}
    if(regModel.password === '' || regModel.password === undefined){return}
    this.disableRegButtton = false
  }

}
