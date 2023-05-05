import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import Swal from 'sweetalert2';
import { AuthService } from './core/service/auth-service';
import { SignalRService } from './core/service/signalR-service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'nobleui-angular';

  constructor(private authService: AuthService,
     private signalRService: SignalRService,
     private router: Router
     ){}

  ngOnInit(): void {
    if(this.authService.getLoggedInUserInfo().sub){
      this.signalRService.makeConnection();
    }
    this.signalRService.profileUpdated.subscribe( () => {
      if(this.signalRService.connection){
        this.signalRService.logout();
      }
      this.authService.logout(this.profileUpdateSweetAlert);
    });
    this.signalRService.profileDeleted.subscribe( () => {
      if(this.signalRService.connection){
        this.signalRService.logout();
      }
      this.authService.logout(this.profileDeletedSweetAlert);
    })
  }

  profileUpdateSweetAlert = () => {
    Swal.fire({
      icon: 'error',
      title: 'Profile Updated!',
      text: 'Your profile has been updated by admin please login again',
      timer: 5000
    });
    this.router.navigate(["auth/login"]);
  }

  profileDeletedSweetAlert = () => {
    Swal.fire({
      icon: 'error',
      title: 'Profile Deleted!',
      text: 'Sorry your profile got deleted from the application',
      timer: 5000
    });
    this.router.navigate(["auth/login"]);
  }
}
