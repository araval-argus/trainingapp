import { Injectable } from "@angular/core";
import * as signalR from '@aspnet/signalr';
import { Subject } from "rxjs";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: "root"
})
export class HubService {
    //Define Connection
    constructor() {
    }
    hubConnection: signalR.HubConnection;

    createConnection() {
        //Initialize Hub Connection
        this.hubConnection = new signalR.HubConnectionBuilder().withUrl(environment.api + "/chatHub", {
            skipNegotiation: true,
            transport: signalR.HttpTransportType.WebSockets
        }).build();
        //Making success function to send 
        const success = () => {
            console.log("Success");
            const token = localStorage.getItem('USERTOKEN');
            this.hubConnection.send("saveConnection", token).then(e => {
                console.log("Connection Saved in Database");
            }
            ).catch(() => {
                console.log("Something Went Wrong");
            })
        }

        //Failure function to handle broken connection
        const fail = () => {
            console.log("Connection Failed");
        }

        //Starting Connection
        this.hubConnection.start().then(success, fail);
    }

    closeConnection() {
        const token = localStorage.getItem('USERTOKEN');
        this.hubConnection.send("deleteConnection", token).then(() => {
            console.log("Connection Deleted from DB");
        }).catch((error) => {
            console.log(error);
        }).finally(() => {
        })
    }



}