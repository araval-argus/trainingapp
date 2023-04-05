import { Component, OnInit } from '@angular/core';
import { TableModule } from 'primeng/table';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';


@Component({
  selector: 'app-new-edit-form',
  templateUrl: './new-edit-form.component.html',
  styleUrls: ['./new-edit-form.component.scss']
})
export class NewEditFormComponent implements OnInit {

  loggedInUser: LoggedInUser[] = [];
  constructor(private tableModule: TableModule, private authService: AuthService) { }

  ngOnInit(): void {
    console.log("inside ngoninit of new edit form component" , this.authService.getLoggedInUserInfo())
    this.loggedInUser.push(this.authService.getLoggedInUserInfo());
  }

}import { RegistrationModel } from 'src/app/core/models/registration-model';
import { AuthService } from 'src/app/core/service/auth-service';

