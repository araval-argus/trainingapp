import { Injectable } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { environment } from "src/environments/environment";
import { RegistrationModel } from "../models/registration-model";
import { LoginModel } from "../models/login-model";

@Injectable({
  providedIn: "root",
})
export class AccountService {
  constructor(private http: HttpClient) {}

  register(registerModel: RegistrationModel) {
    return this.http.post( environment.apiUrl + "/account/register", registerModel);
  }

  login(loginModel: LoginModel) {
    loginModel.userName = loginModel.emailAddress;
    return this.http.post( environment.apiUrl + "/account/login", loginModel);
  }

  update(formdata: FormData) {
    return this.http.post( environment.apiUrl + "/account/update-profile", formdata);
  }

  getAllStatus() {
    return this.http.get( environment.apiUrl + "/account/GetStatusList");
  }

  getUserStatus(userName: string) {
    return this.http.get( environment.apiUrl + "/account/Status/" + userName);
  }

  changeStatus(statusCode: number, userName: string) {
    return this.http.post( environment.apiUrl + "/account/ChangeStatus/" + userName,statusCode);
  }

  getAllUsers() {
    return this.http.get( environment.apiUrl + "/account/GetUsers");
  }

  getAllDesignation() {
    return this.http.get( environment.apiUrl + "/admin/GetDesignation");
  }

  DeleteUser(userName: string) {
    return this.http.delete( environment.apiUrl + "/admin/DeleteUser/" + userName);
  }

  updateByAdmin(formdata: FormData) {
    return this.http.post( environment.apiUrl + "/admin/UpdateProfile",formdata);
  }

  changePassword(password:any){
    return this.http.post( environment.apiUrl + "/account/ChangePassword" , password)
  }
}
