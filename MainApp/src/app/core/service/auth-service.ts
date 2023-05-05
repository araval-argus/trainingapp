import { Injectable } from "@angular/core";
import { JwtHelper } from "../helper/jwt-helper";
import { LoggedInUserModel } from "../models/loggedin-user";
import { environment } from "src/environments/environment";
import { Subject } from 'rxjs';

@Injectable({
  providedIn: "root",
})
export class AuthService {
  constructor(private jwtHelper: JwtHelper) {}

  LoggedInUserChanged = new Subject();

  login(token, callback?) {
    localStorage.setItem("isLoggedin", "true");
    localStorage.setItem("USERTOKEN", token);

    if (callback) {
      callback();
    }
    this.LoggedInUserChanged.next();
  }
  logout(callback?) {
    localStorage.removeItem("isLoggedin");
    localStorage.removeItem("USERTOKEN");
    if (callback) {
      callback();
    }
  }

  getLoggedInUserInfo() {
    let token = localStorage.getItem("USERTOKEN");
    var user: LoggedInUserModel = this.jwtHelper.decodeToken(token);
    user.imageUrl = environment.apiUrl + "/../Images/Users/" + user.imageUrl
    return user;
  }

  getUserToken() {
    return localStorage.getItem("USERTOKEN");
  }


}
