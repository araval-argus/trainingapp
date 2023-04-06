import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
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

    update(formData: FormData){
        return this.http.post(environment.apiUrl + "/account/update", formData);
    }

    checkUsername(username: string){
      return this.http.get<{ [key: string]: boolean } >(environment.apiUrl + "/account/checkUsername",
      {
        params: new HttpParams().append('username', username)
      });
    }


    dummyRequest(formData: FormData){
      return this.http.post(environment.apiUrl + "/account/dummy", formData);
    }
}
