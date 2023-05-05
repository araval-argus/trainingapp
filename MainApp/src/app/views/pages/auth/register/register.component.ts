import { Component, OnInit } from '@angular/core';
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

  //Type List
  typeList: any[] = [{
    id: 2, name: 'CEO'
  }, {
    id: 3, name: 'CTO'
  }, {
    id: 4, name: 'GroupLead'
  }, {
    id: 5, name: 'SolutionAnalyst'
  }, {
    id: 6, name: 'ProgramAnalyst'
  }, {
    id: 7, name: 'Probationer'
  }, {
    id: 8, name: 'Intern'
  }];

  constructor(private router: Router,
    private accountService: AccountService,
    private authService: AuthService) { }

  ngOnInit(): void {
    this.accountService.getRoles().subscribe(data => {
      console.log(data);

      if (data[0]) {
        this.typeList = this.typeList.filter(e => e.id != 2);
      }
      if (data[1]) {
        this.typeList = this.typeList.filter(e => e.id != 3);
      }
    })

    console.log(this.typeList);
    this.regModel = {
      firstName: '',
      lastName: '',
      userName: '',
      email: '',
      password: '',
      type: 8
    }
  }

  onRegister(e) {
    e.preventDefault();
    console.log(this.regModel);

    this.disableRegButtton = true;
    this.accountService.register(this.regModel)
      .subscribe((data: any) => {
        this.authService.login(data.token, () => {
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
        console.log(err);
        Swal.fire({
          title: 'Error!',
          text: err.error.message,
          icon: 'error',
        });
      });

  }

}
