import { Injectable } from "@angular/core";
import * as signalR from "@microsoft/signalr";
import { environment } from "src/environments/environment";

@Injectable({
  providedIn:'root'
})

export class SignalRService{

  constructor() {  }

  hubConnection : signalR.HubConnection

  startConnection = (userName : string) =>{
    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(environment.hubUrl,{
      skipNegotiation:true,
      transport : signalR.HttpTransportType.WebSockets
    })
    .withAutomaticReconnect()
    .build();

    this.hubConnection.start()
    .then(()=>{

      console.log('hub Connection Started ' + userName);

      this.hubConnection.invoke("ConnectDone",userName)
      .catch((error)=>{console.log(error)});

    }).catch(error=>{console.log(error)})

  }
}


