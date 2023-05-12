import { Injectable } from "@angular/core";
import * as signalR from "@microsoft/signalr";
import { environment } from "src/environments/environment";
import Swal from "sweetalert2";

@Injectable({
  providedIn:'root'
})

export class SignalRService{

  constructor() {}

  hubConnection : signalR.HubConnection

  startConnection = (userName : string) =>{
    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(environment.hubUrl,{
      skipNegotiation:true,
      transport : signalR.HttpTransportType.WebSockets
    })
    .withAutomaticReconnect()
    .build();

    this.hubConnection.on("myProfileChanged", (response: string) => {
      console.log("Hi");
      localStorage.setItem("USERTOKEN", response);
      setTimeout(()=>{window.location.reload()},3050)
      Swal.fire({
        title: "Success!",
        text: "Your Profile has been Changed by Admin.",
        icon: "success",
        timer: 3000,
        timerProgressBar: true,
      });
    });

    this.hubConnection.start()
    .then(()=>{

      console.log('hub Connection Started ' + userName);

      this.hubConnection.invoke("ConnectDone",userName)
      .catch((error)=>{console.log(error)});

    }).catch(error=>{console.log(error)})
  }
}

