import { Component, OnInit } from '@angular/core';
import { HubService } from './core/service/hub-service';
import { AuthService } from './core/service/auth-service';
import Swal from 'sweetalert2';
import { Router } from '@angular/router';
import { AccountService } from './core/service/account-service';
import { EmployeeService } from './core/service/employee-service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'nobleui-angular';

  constructor(private hubService: HubService, private authService: AuthService, private router: Router, private accountService: AccountService, private employeeService: EmployeeService) { }
  ngOnInit(): void {
    if (localStorage.getItem('isLoggedin')) {

      //On reaload create new connection
      this.hubService.createConnection();


      //logged In user updated
      this.hubService.hubConnection.on("loggedInUserDeleted", () => {
        this.accountService.setStatus(6).subscribe(data => { });
        this.authService.logout(() => {
          Swal.fire({
            title: 'Deleted!',
            text: 'You are removed from Organization.',
            icon: 'error',
            timer: 3000,
            timerProgressBar: true,
          });
          this.router.navigate(['/auth/login']);
        });
      })

      //loggedInUserUpdated
      this.hubService.hubConnection.on("loggedInUserUpdated", (token) => {
        localStorage.setItem('USERTOKEN', token);
        this.employeeService.roleUpdated.next();
      })
    }
  }

}
