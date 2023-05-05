import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from "@angular/core";
import { environment } from 'src/environments/environment';
import { UserModel } from '../models/UserModel';

@Injectable({
  providedIn: "root"
})
export class AdminService{
  constructor(private http: HttpClient){}

  fetchEmployees(){
    return this.http.get(environment.apiUrl + "/admin/FetchAllEmployees");
  }

  deleteEmployee(userName: string){
    console.log(userName)
    return this.http.patch(environment.apiUrl + "/admin/DeleteEmployee", {userName}, {
      headers: new HttpHeaders().set('Content-type', 'application/json')
    });
  }

  updateEmployeeData(employee: UserModel, userName: string){
    return this.http.patch(environment.apiUrl + "/admin/UpdateEmployeeData", employee, {
      params: new HttpParams().append("employeeOldUsername", userName)
    });
  }

}
