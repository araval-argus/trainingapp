import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class messageDataService {
  constructor(private http: HttpClient){

  }

  fetchInsights(){
    return this.http.get(environment.apiUrl + "/values/fetchInsights");
  }
}
