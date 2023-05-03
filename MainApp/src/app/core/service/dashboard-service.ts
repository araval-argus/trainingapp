import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: 'root'
})

export class DashBoardService{

  constructor(private http: HttpClient) { }

  getChartDetails(){
    return this.http.get(environment.apiUrl + "/DashBoard/Chart");
  }

}
