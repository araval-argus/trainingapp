import { EventEmitter, Injectable } from "@angular/core";
import { JwtHelper } from "../helper/jwt-helper";
import { LoggedInUser } from "../models/loggedin-user";
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  constructor(private jwtHelper: JwtHelper) {}

  loggedInUserChanged = new EventEmitter();

  login(token, callback) {
    localStorage.setItem("isLoggedin", "true");
    localStorage.setItem("USERTOKEN", token);

    if (callback) {
      callback();
    }
    this.loggedInUserChanged.emit();
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
    var user: LoggedInUser = this.jwtHelper.decodeToken(token);
    user.imageUrl = environment.apiUrl + "/../Images/Users/" + user.imageUrl
    user.designation = this.setDesignation(user.designation)
    //console.log("user inside getLoggedInUserInfo:- ", user)
    return user;
  }

  getUserToken() {
    return localStorage.getItem("USERTOKEN");
  }

  setDesignation(id: string) {
    switch (id) {
      case "2":
        return "Programmer Analyst";
      case "3":
        return "Solution Analyst";
      case "4":
        return "Lead Solution Analyst";
      case "5":
        return "Intern";
      case "6":
        return "Probationer";
      case "7":
        return "Quality Analyst";
    }
  }
}
