import { Component, OnInit, Inject, Renderer2, TemplateRef } from '@angular/core';
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
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

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
    private modalService: NgbModal
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

  seenAll() {
    this.notificationService.markAsSeen().subscribe(data => {
      this.notifications.forEach(e => e.isSeen = 1);
      console.log(data);
      this.newCount = 0;
      console.log(this.newCount);
    })
  }

  viewAll(content: TemplateRef<any>) {
    this.modalService.open(content, {});
  }

  view(id: number) {
    this.notificationService.view(id).subscribe((data: any) => {
      if (data.notification) {
        var curNotification = this.notifications.find(e => e.id == id);
        curNotification.isSeen = 1;
        this.newCount--;
      }
    })
  }

  deleteAll() {
    this.notificationService.deleteAll().subscribe(data => {
      this.notifications = [];
    })
  }


  delete(id: number) {
    this.notificationService.delete(id).subscribe((data: any) => {
      if (data.notification) {
        this.notifications = this.notifications.filter(e => e.id != id);
        this.newCount--;
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
    this.accountService.setStatus(6).subscribe(data => { });
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
