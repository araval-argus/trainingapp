import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from "src/environments/environment";
import { RegistrationModel } from "../models/registration-model";
import { LoginModel } from "../models/login-model";
import { Subject } from "rxjs";
import { LoggedInUser } from "../models/loggedin-user";

@Injectable({
    providedIn: 'root'
})
export class AccountService {

    public updateDetails = new Subject();
    constructor(private http: HttpClient) { }

    register(registerModel: RegistrationModel) {
        return this.http.post(environment.apiUrl + "/account/register", registerModel);
    }

    login(loginModel: LoginModel) {
        loginModel.userName = loginModel.emailAddress;
        return this.http.post(environment.apiUrl + "/account/login", loginModel);
    }

    update(formData: FormData, username: string) {
        return this.http.put(environment.apiUrl + "/account/update/" + username, formData)
    }

    getImage(username: string) {
        return this.http.get(environment.apiUrl + "/account/getImage?username=" + username);
    }

    getImageUrl(imageName: string) {
        return environment.api + "/images/" + imageName;
    }
}
