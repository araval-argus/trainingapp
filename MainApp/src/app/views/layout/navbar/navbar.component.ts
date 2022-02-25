import { Component, OnInit, ViewChild, ElementRef, Inject, Renderer2 } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/core/service/auth-service';
import Swal from 'sweetalert2'
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { UserService } from 'src/app/core/service/user.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {

  loggedInUser: LoggedInUser

  userObj: LoggedInUser;

  // some placeholder image
  profileUrl: string = "https://via.placeholder.com/30x30";

  constructor(
    @Inject(DOCUMENT) private document: Document,
    private renderer: Renderer2,
    private router: Router,
    private authService: AuthService,
    private userService: UserService
  ) { }

  async ngOnInit(): Promise<void> {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    console.log(this.loggedInUser);

    if (!this.userService.currentUserObject) {
      await this.userService.setCurrentUserSubject();
    }
 
    this.userService.currentUserObject.subscribe(
      userObj => {  
        this.userObj = userObj; 

        if (userObj.profileUrl != environment.hostUrl + '/') {
          this.profileUrl = userObj.profileUrl+'?'+new Date() 
        }
    })

  }

  /**
   * Sidebar toggle on hamburger button click
   */
  toggleSidebar(e) {
    e.preventDefault();
    this.document.body.classList.toggle('sidebar-open');
  }

  /**
   * Logout
   */
  onLogout(e) {
    e.preventDefault();
    this.authService.logout(() => {
      Swal.fire({
        title: 'Success!',
        text: 'User has been logged out.',
        icon: 'success',
        timer: 2000,
        timerProgressBar: true,
      });
      this.router.navigate(['/auth/login']);
    });

  }

}
