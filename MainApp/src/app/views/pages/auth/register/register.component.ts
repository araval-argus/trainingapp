import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
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

  regForm: FormGroup;


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

    this.regForm = new FormGroup({
      firstName: new FormControl('', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]),
      lastName: new FormControl('', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]),
      userName: new FormControl('',  [Validators.required, Validators.minLength(2), Validators.maxLength(50)]),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required, Validators.minLength(8)])
    })

  }

  onRegister(e) {
    e.preventDefault();
    console.log(this.regForm.value);
    this.disableRegButtton = true; 
    this.accountService.register(this.regForm.value)
      .subscribe((data: any) => {
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
            this.disableRegButtton = false; 

          }, (3000));
        })

      }, (err) => {
        this.disableRegButtton = false; 
        Swal.fire({
          title: 'Error!',
          text: err.error.message,
          icon: 'error',
        });
      });

  }

}
