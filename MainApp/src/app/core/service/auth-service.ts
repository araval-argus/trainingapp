import { EventEmitter, Injectable } from "@angular/core";
import { JwtHelper } from "../helper/jwt-helper";
import { LoggedInUserModel } from "../models/loggedin-user";
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  constructor(private jwtHelper: JwtHelper) {}

  LoggedInUserChanged = new EventEmitter();

  login(token, callback) {
    localStorage.setItem("isLoggedin", "true");
    localStorage.setItem("USERTOKEN", token);

    if (callback) {
      callback();
    }
    this.LoggedInUserChanged.emit();
  }
  logout(callback) {
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
    //console.log(user);
    //user.designation = this.setDesignation(user.designation)
    //console.log("user inside getLoggedInUserModelInfo:- ", user)
    return user;
  }

  getUserToken() {
    return localStorage.getItem("USERTOKEN");
  }


}
