import { Component, OnInit, Inject, Renderer2 } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/core/service/auth-service';
import Swal from 'sweetalert2'
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { AccountService } from 'src/app/core/service/account-service';
import { HubService } from 'src/app/core/service/hub-service';
import { notificationModel } from 'src/app/core/models/notification-model';
import { NotificationService } from 'src/app/core/service/notification-service';
import { GroupService } from 'src/app/core/service/group-service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  loggedInUser: LoggedInUser
  imageURL: string = localStorage.getItem('imagePath');
  notifications: notificationModel[];
  newCount: number = 0;
  constructor(
    @Inject(DOCUMENT) private document: Document,
    private renderer: Renderer2,
    private router: Router,
    private authService: AuthService,
    private accountService: AccountService,
    private hubService: HubService,
    private notificationService: NotificationService,
    private groupService: GroupService
  ) { }

  ngOnInit(): void {
    this.notifications = [];
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.accountService.updateDetails.subscribe(() => {
      this.loggedInUser = this.authService.getLoggedInUserInfo();
      this.imageURL = localStorage.getItem('imagePath');
    })

    this.hubService.hubConnection.on("newNotification", (data: any) => {
      this.pushNotification(data);
    })

    this.notificationService.getAll().subscribe((data: any) => {
      data.notifications.forEach(element => {
        this.pushNotification(element);
      });
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

  openNotification(notification: notificationModel) {
    if (notification.isGroup == 1) {
      this.groupService.openFromNotification.next(notification.content);
    }
  }

  pushNotification(notification: any) {
    this.notifications.push({
      id: notification.id,
      content: notification.content,
      isGroup: notification.isGroup,
      isSeen: notification.isSeen,
      time: new Date(notification.time)
    })
    this.newCount = this.notifications.filter(e => e.isSeen == 0).length;
    this.notifications.sort((a, b) => b.time.getTime() - a.time.getTime())
  }
}
