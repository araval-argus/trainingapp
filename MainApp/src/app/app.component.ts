import { Component, OnInit } from '@angular/core';
import { SignalRService } from './core/service/signalr-service';
import { AuthService } from './core/service/auth-service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'nobleui-angular';

  constructor( private signalRService : SignalRService , private authService : AuthService){}

  ngOnInit(): void {
    this.signalRService.startConnection(this.authService.getLoggedInUserInfo().userName);
  }

}
