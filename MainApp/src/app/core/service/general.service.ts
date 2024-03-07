import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({providedIn: 'root'})
export class GeneralService {
    
    constructor(private http : HttpClient) { }
    
    getAreas() {
        return this.http.get(environment.apiUrl + "/General/areas");
    }

    createPost(post:any) {
        return this.http.post(environment.apiUrl + "/General/CreateGeneralPost",post);
    }

    getAllMessages(areaId:number){
        return this.http.get(environment.apiUrl + "/General/getAllMessages", {
            params : new HttpParams().append('AreaId', areaId)
        })
    }
}