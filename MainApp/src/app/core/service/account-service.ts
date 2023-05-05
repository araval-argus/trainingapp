import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from "src/environments/environment";
import { RegistrationModel } from "../models/registration-model";
import { LoginModel } from "../models/login-model";
import { PasswordModel } from '../models/password-model';

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

    checkUsername(username: string) {
      return this.http.get<{ [key: string]: boolean } >(environment.apiUrl + "/account/checkUsername",
      {
        params: new HttpParams().append('username', username)
      });
    }

    fetchDesignations(){
      return this.http.get(environment.apiUrl+ "/account/fetchDesignations");
    }

    checkCurrPassword(username: string, currPassword: string){
      let loginModel: LoginModel = {
        userName : username,
        password: currPassword,
        emailAddress: username
      }
      return this.http.post(environment.apiUrl + "/account/checkPassword", loginModel);
    }

    changePassword(passwordModel: PasswordModel){
      return this.http.post(environment.apiUrl + "/account/changePassword",passwordModel);
    }

    //fetches all other users except logged in user
    fetchAllUsers(loggedInUsername: string){
      return this.http.get(environment.apiUrl + "/account/FetchAllUsers", {
        params: new HttpParams().append("userName", loggedInUsername)
      });
    }
}
