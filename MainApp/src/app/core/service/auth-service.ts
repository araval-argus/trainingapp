import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { JwtHelper } from "../helper/jwt-helper";
import { LoggedInUser } from "../models/loggedin-user";

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    constructor(private jwtHelper: JwtHelper, private httpClient: HttpClient) {

    }
    login(token, callback) {
        localStorage.setItem('isLoggedin', 'true');
        localStorage.setItem('USERTOKEN', token);
        if (callback) {
            callback();
        }

    }
    logout(callback) {

        this.httpClient.post(environment.apiUrl + "/Account/logout", {}).subscribe()
        console.log("Logout");
        
        localStorage.removeItem('isLoggedin');
        localStorage.removeItem('USERTOKEN');
        localStorage.removeItem('authToken');
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
