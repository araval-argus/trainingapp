import { Injectable } from "@angular/core";
import { JwtHelper } from "../helper/jwt-helper";
import { LoggedInUser } from "../models/loggedin-user";
import { AccountService } from "./account-service";
import { HubService } from "./hub-service";

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    constructor(private jwtHelper: JwtHelper, private hubService: HubService) {

    }
    login(token, callback) {
        localStorage.setItem('isLoggedin', 'true');
        localStorage.setItem('USERTOKEN', token);
        localStorage.setItem('statusId', '1');
        this.hubService.createConnection();
        if (callback) {
            callback();
        }

    }
    logout(callback) {
        this.hubService.closeConnection();
        localStorage.removeItem('isLoggedin');
        localStorage.removeItem('USERTOKEN');
        localStorage.removeItem('imagePath');
        localStorage.removeItem('statusId');
        if (callback) {
            callback();
        }
    }

    getLoggedInUserInfo() {
        let token = localStorage.getItem('USERTOKEN');
        var user: LoggedInUser = this.jwtHelper.decodeToken(token);
        return user;
    }

    getUserToken() {
        return localStorage.getItem('USERTOKEN');
    }
}
