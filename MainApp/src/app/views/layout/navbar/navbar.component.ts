import { Component, OnInit, ViewChild, ElementRef, Inject, Renderer2, OnDestroy } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/core/service/auth-service';
import Swal from 'sweetalert2'
import { LoggedInUserModel } from 'src/app/core/models/loggedin-user';
import { environment } from 'src/environments/environment';
import { SignalRService } from 'src/app/core/service/signalR-service';
import { NotificationService } from 'src/app/core/service/notification.service';
import { NotificationModel } from 'src/app/core/models/Notification-model';
import { Subscription } from 'rxjs';
import { AccountService } from 'src/app/core/service/account-service';

@Component({
  selector: "app-navbar",
  templateUrl: "./navbar.component.html",
  styleUrls: ["./navbar.component.scss"],
})
export class NavbarComponent implements OnInit, OnDestroy {
  today: Date = new Date();
  loggedInUser: LoggedInUserModel;
  loggedInUserStatusId: number;
  apiUrl: string = environment.apiUrl;
  notifications: NotificationModel[];
  subscriptions: Subscription[] = [];
  allStatus: {
    id: number,
    type: string,
    tagClasses: string,
    tagStyle: string
  }[] = [];

  constructor(
    @Inject(DOCUMENT) private document: Document,
    private renderer: Renderer2,
    private router: Router,
    private authService: AuthService,
    private signalRService: SignalRService,
    private notificationService: NotificationService,
    private accountService: AccountService
  ) {}

  ngOnInit(): void {
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.fetchLoggedInUserStatus();
    this.removeNotificationsSubscription();
    this.subscibeToFetchNotifications();
    this.subscribeToAddNotification();
    this.subscribeToChangedLoggedInUser();
    this.fetchAllStatus();
  }

  /**
   * Sidebar toggle on hamburger button click
   */
  toggleSidebar(e) {
    e.preventDefault();
    this.document.body.classList.toggle("sidebar-open");
  }

  /**
   * Logout
   */
  onLogout(e) {
    e.preventDefault();
    if (this.signalRService.connection) {
      this.signalRService.logout();
      //this.signalRService.stopConnection();
    }
    this.authService.logout(() => {
      Swal.fire({
        title: "Success!",
        text: "User has been logged out.",
        icon: "success",
        timer: 2000,
        timerProgressBar: true,
      });
      this.router.navigate(["/auth/login"]);
    });
  }

  clearAllNotification() {
    const sub = this.notificationService
      .clearAllNotification()
      .subscribe(() => {
        this.notifications.splice(0, this.notifications.length);
      });
    this.subscriptions.push(sub);
  }

  removeNotificationsSubscription() {
    const sub = this.signalRService.notificationRemoved.subscribe(
      (userOrGroup: string) => {
        let notificationsToBeRemoved = this.notifications.filter(
          (notification) =>
            notification.raisedBy === userOrGroup ||
            notification.raisedInGroup === userOrGroup
        );
        notificationsToBeRemoved.forEach( notification => this.notifications.splice(this.notifications.indexOf(notification),1));
      }
    );
    this.subscriptions.push(sub);
  }

  subscibeToFetchNotifications() {
    this.notificationService
      .fetchNotifications()
      .subscribe((notifications: NotificationModel[]) => {
        notifications.forEach((n) => {
          n.createdAt = new Date(n.createdAt);
          n.messageToBeDisplayed = this.setMessage(n);
        });
        this.notifications = notifications;
      });
  }

  subscribeToAddNotification() {

    const sub = this.signalRService.addNotification.subscribe(
      (notification: NotificationModel) => {
        notification.createdAt = new Date(notification.createdAt);
        notification.messageToBeDisplayed = this.setMessage(notification);
        this.notifications.splice(0, 0, notification);
      }
    );
    this.subscriptions.push(sub);
  }

  subscribeToChangedLoggedInUser() {
    const sub = this.authService.LoggedInUserChanged.subscribe(() => {
      this.loggedInUser = this.authService.getLoggedInUserInfo();
    });
    this.subscriptions.push(sub);
  }

  fetchAllStatus(){
    this.accountService.fetchAllStatus().subscribe( (allStatus: any) => {
      this.allStatus = allStatus;
    });
  }

  fetchLoggedInUserStatus(){
    this.accountService.fetchLoggedInUserStatus().subscribe( (status: any) => {
      this.loggedInUserStatusId = status;
    })
  }

  compareFn(item, selected){
    return item == selected;
  }

  changeStatus(){
    console.log(this.loggedInUserStatusId)
    this.accountService.changeStatus(this.loggedInUserStatusId).subscribe( () => {
    });
  }

  setMessage(notificationModel: NotificationModel) {
    switch (notificationModel.type.id) {
      case 1:
        return notificationModel.raisedBy + " sent you a message";
      case 2:
        return notificationModel.raisedBy + " sent you an image";
      case 3:
        return notificationModel.raisedBy + " sent you a video";
      case 4:
        return notificationModel.raisedBy + " sent you an audio";
      case 5:
        return (
          notificationModel.raisedBy +
          " sent a message in " +
          notificationModel.raisedInGroup
        );
      case 6:
        return (
          notificationModel.raisedBy +
          " sent an image in " +
          notificationModel.raisedInGroup
        );
      case 7:
        return (
          notificationModel.raisedBy +
          " sent a video in " +
          notificationModel.raisedInGroup
        );
      case 8:
        return (
          notificationModel.raisedBy +
          " sent an audio in " +
          notificationModel.raisedInGroup
        );
      case 9:
        return (
          notificationModel.raisedBy +
          " left " +
          notificationModel.raisedInGroup
        );
      case 10:
        return (
          notificationModel.raisedBy +
          " removed a user from " +
          notificationModel.raisedInGroup
        );
      case 11:
        return (
          notificationModel.raisedBy +
          " added a new user into " +
          notificationModel.raisedInGroup
        );
      case 12:
        return (
          notificationModel.raisedBy +
          " created a new admin for " +
          notificationModel.raisedInGroup
        );
    }
  }

  ngOnDestroy() {
    this.subscriptions.forEach((subscription) => subscription.unsubscribe());
  }
}
