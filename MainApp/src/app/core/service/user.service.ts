import { HttpClient, HttpEvent } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LoggedInUser } from '../models/loggedin-user';

import { AuthService } from './auth-service';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  
  // currentUser: LoggedInUser;
  
  // behavior subjects
  private _currentUserSubject: BehaviorSubject<LoggedInUser>;

  currentUserObject: Observable<LoggedInUser>; 

  
 constructor(private httpClient: HttpClient, private authService: AuthService) {
    this.getCurrentUserDetails().subscribe(
      (response) => {
        this._currentUserSubject = new BehaviorSubject<any>(response);
        this.currentUserObject = this._currentUserSubject.asObservable(); 
      }
    )

  }
  
  getCurrentUserDetails(): Observable<LoggedInUser> {
    const user = this.authService.getLoggedInUserInfo();
    console.log("-------------");
    console.log(user);
    return this.httpClient.get<LoggedInUser>(environment.apiUrl + "/user/"+user.sub);
  }

  getUsers(): Observable<LoggedInUser[]> {
    return this.httpClient.get<LoggedInUser[]>(environment.apiUrl + "/user/all")
  }

  updateUserDetails(userObj: any) {

    const username = this.authService.getLoggedInUserInfo().sub;

    return this.httpClient.put<LoggedInUser>(environment.apiUrl + "/user/"+username, userObj);
  }

  updateUserProfileImage(profileObj: any): Observable<HttpEvent<{filePath: string}>> {
    const username = this.authService.getLoggedInUserInfo().sub;

    return this.httpClient.post<{filePath: string}>(environment.apiUrl + "/user/" + username + "/profileUpload", profileObj, {
      reportProgress: true,
      observe: 'events'
    });
  }

  getUserProfileUrl(): Observable<{profileUrl: string}> {
    const username = this.authService.getLoggedInUserInfo().sub;
    return this.httpClient.get<{profileUrl: string}>(environment.apiUrl + "/user/" + username + "/getProfileUrl")
  }

  searchUser(user: string): Observable<LoggedInUser[]> {
    return this.httpClient.get<LoggedInUser[]>(environment.apiUrl + "/user/search/" + user);
  }

  getUserByUserName(username: string) {
    return this.httpClient.get<LoggedInUser>(environment.apiUrl + "/user/"+ username);
  }

  // setting user behavior object

  async setCurrentUserSubject() {
    await this.getUserAsPromise().then(
      (response) => {
        response.profileUrl = environment.hostUrl + '/' + response.profileUrl;
        this._currentUserSubject.next(response);
      }
    )
  }

  private getUserAsPromise(): Promise<LoggedInUser> {
    const username = this.authService.getLoggedInUserInfo().sub;
    return this.httpClient.get<LoggedInUser>(environment.apiUrl + "/user/" + username).toPromise();
  }

  updateUserObject(user: LoggedInUser) {
    this._currentUserSubject.next(user);
  }

  // user behavior object set complete 

}
