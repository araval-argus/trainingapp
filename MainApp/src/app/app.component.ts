import { Component, OnInit } from '@angular/core';
import { AuthService } from './core/service/auth-service';
import { SignalRService } from './core/service/signalR-service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'nobleui-angular';

  constructor(private authService: AuthService, private signalRService: SignalRService){

  }

  ngOnInit(): void {
    if(this.authService.getLoggedInUserInfo().sub){
      this.signalRService.makeConnection();
    }
  }
}
