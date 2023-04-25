import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: "root"
})
export class NotificationService {
    constructor(private http: HttpClient) { }
    getAll() {
        return this.http.get(environment.apiUrl + "/Notification/getAll");
    }
    markAsSeen() {
        return this.http.get(environment.apiUrl + "/Notification/markAsSeen");
    }
    deleteAll() {
        return this.http.get(environment.apiUrl + "/Notification/deleteAll");
    }
    delete(id: number) {
        return this.http.get(environment.apiUrl + "/Notification/delete?id=" + id);
    }
    view(id: number) {
        return this.http.get(environment.apiUrl + "/Notification/view?id=" + id)
    }
}