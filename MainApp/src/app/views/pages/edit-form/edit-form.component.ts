import { Component, EventEmitter, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import Swal from 'sweetalert2'

@Component({
  selector: 'app-edit-form',
  templateUrl: './edit-form.component.html',
  styleUrls: ['./edit-form.component.scss']
})
export class EditFormComponent implements OnInit {

  constructor(private authService: AuthService,
      private accountService: AccountService,
      private router : Router
    ) { }


    loggedInUserChanged = new EventEmitter();;

  loggedInUser : LoggedInUser;

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.loggedInUser.userName = this.loggedInUser.sub;
  }

  onUpdate(e: Event){
    e.preventDefault();
    console.log("in onUpdate method ", this.loggedInUser);
    this.accountService.update(this.loggedInUser).subscribe(
      (data : any )=> {
        console.log(data);
        this.authService.login(data.token, ()=>{
          Swal.fire({
            title: 'Success!',
            text: 'Details has been updated',
            icon: 'success',
            timer: 2000,
           timerProgressBar: true,
          });
          setTimeout(() => {
            console.log("navigating to dashboard after updating....")
            this.loggedInUserChanged.emit();
            this.router.navigate(["/"]);

          }, (3000));
        })

      },
      err => {
        console.log(err);
      }
    )
  }
}
