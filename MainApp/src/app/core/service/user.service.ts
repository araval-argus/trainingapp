import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LoggedInUser } from '../models/loggedin-user';

import { HttpHeaders } from '@angular/common/http';
import { AuthService } from './auth-service';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  
  constructor(
    private httpClient: HttpClient,
    private authService: AuthService
    ) { }
  
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

}
