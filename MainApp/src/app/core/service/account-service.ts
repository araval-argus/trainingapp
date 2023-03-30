import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { environment } from "src/environments/environment";
import { RegistrationModel } from "../models/registration-model";
import { LoginModel } from "../models/login-model";
import { LoggedInUser } from "../models/loggedin-user";

@Injectable({
    providedIn: 'root'
})
export class AccountService {
    constructor(private http: HttpClient) { }



    register(registerModel: RegistrationModel) {
        return this.http.post(environment.apiUrl + "/account/register", registerModel);
    }

    login(loginModel: LoginModel) {
        loginModel.userName = loginModel.emailAddress;
        return this.http.post(environment.apiUrl + "/account/login", loginModel);
    }

    update(loggedInUser: LoggedInUser){
        return this.http.post(environment.apiUrl + "/account/update", loggedInUser);
    }
}
