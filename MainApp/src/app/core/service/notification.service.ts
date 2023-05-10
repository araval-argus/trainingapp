import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { NotificationModel } from '../models/Notification-model';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  notifications: NotificationModel[];

  constructor(private http: HttpClient) { }

  fetchNotifications(){
    return this.http.get<NotificationModel[]>(environment.apiUrl + "/notification/fetchNotifications");
  }

  clearAllNotification(){
    return this.http.delete(environment.apiUrl+ "/notification/clearAllNotifications")
  }

}
