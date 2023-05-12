import { Component, OnInit , Inject } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/core/service/auth-service';
import Swal from 'sweetalert2'
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { environment } from 'src/environments/environment';
import { SignalRService } from 'src/app/core/service/signalr-service';
import { NotificationService } from 'src/app/core/service/notification-service';
import { NotificationsModel } from 'src/app/core/models/notifications-model';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})

export class NavbarComponent implements OnInit {

  loggedInUser: LoggedInUser;
  environment = environment.ImageUrl;
  notifications : NotificationsModel[] = [];

  constructor(
    @Inject(DOCUMENT) private document: Document, private router: Router, private authService: AuthService,
    private signalRService : SignalRService,private notificationService: NotificationService, private modalService: NgbModal) { }

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.authService.UserProfileChanged.subscribe(()=>{
      this.loggedInUser = this.authService.getLoggedInUserInfo();
    });

    this.notificationService.getNotifications(this.loggedInUser.userName).subscribe((data:NotificationsModel[])=>{
      this.notifications = data;
    });

    this.signalRService.hubConnection.on('notification',(data:NotificationsModel)=>{
      const index = this.notifications.findIndex(u => u.msgFrom==data.msgFrom && u.msgTo==data.msgTo && u.groupId==data.groupId);
      if (index !== -1) {
        this.notifications.splice(index,1);
      }
      this.notifications.splice(0,0,data);
    });
  }

  onClearAll(){
    this.notifications = [];
    this.notificationService.clearNotifications(this.loggedInUser.userName).subscribe();
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
    this.signalRService.hubConnection.invoke("ConnectRemove",this.loggedInUser.userName).then(()=>{
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
    })
  }

  openScrollableModal(content) {
    this.modalService.open(content, {scrollable: true}).result.then((result) => {
      console.log("Modal closed" + result);
    }).catch((res) => {});
  }

}
