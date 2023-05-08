import { Injectable } from "@angular/core";
import { JwtHelper } from "../helper/jwt-helper";
import { LoggedInUser } from "../models/loggedin-user";
import { Subject } from "rxjs";
import { SignalRService } from "./signalr-service";

@Injectable({
    providedIn: 'root'
})

export class AuthService {
    constructor(private jwtHelper: JwtHelper , private signalRService : SignalRService) {}

    UserProfileChanged = new Subject<LoggedInUser>();

    login(token, callback) {
        localStorage.setItem('isLoggedin', 'true');
        localStorage.setItem('USERTOKEN', token);
        if (callback) {
            callback();
        }
    }
    logout(callback) {
        localStorage.removeItem('isLoggedin');
        localStorage.removeItem('USERTOKEN');
        this.signalRService.hubConnection?.stop().catch(error=>{console.log(error)});
        if (callback) {
            callback();
        }
    }

    getLoggedInUserInfo() {
        let token = localStorage.getItem('USERTOKEN');
        var user: LoggedInUser = this.jwtHelper.decodeToken(token);
        user.userName = user.sub;
        console.log(user);
        return user;
    }

    getUserToken() {
        return localStorage.getItem('USERTOKEN');
    }
}
