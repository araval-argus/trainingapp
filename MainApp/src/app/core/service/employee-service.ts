import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { RegistrationModel } from "../models/registration-model";
import { Subject } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class EmployeeService {

    public roleUpdated = new Subject();

    constructor(private http: HttpClient) { }
    getAllEmployee() {
        return this.http.get(environment.apiUrl + "/employee/getAll");
    }

    updateRole(user: string, profileType: number) {
        let queryParams = new HttpParams();
        queryParams = queryParams.append("userName", user);
        queryParams = queryParams.append("profileType", profileType.toString());
        return this.http.get(environment.apiUrl + "/employee/updateRole", { params: queryParams })
    }

    deleteUser(userName: string) {
        return this.http.get(environment.apiUrl + "/employee/deleteUser?userName=" + userName)
    }

    addUser(regModel: RegistrationModel) {
        return this.http.post(environment.apiUrl + "/employee/addUser", regModel);
    }
}