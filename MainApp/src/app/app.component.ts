import { Component, OnInit } from '@angular/core';
import { SignalRService } from './core/service/signalR-service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'nobleui-angular';

  constructor(private signalRService: SignalRService){}
  ngOnInit(): void {
    this.signalRService.makeConnection();
  }

}
