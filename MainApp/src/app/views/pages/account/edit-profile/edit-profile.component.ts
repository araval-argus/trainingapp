import { HttpEventType } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Form, FormControl, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { UserService } from 'src/app/core/service/user.service';
import { environment } from 'src/environments/environment';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-edit-profile',
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.scss']
})
export class EditProfileComponent implements OnInit {


  userObj: LoggedInUser;
  editForm: FormGroup;

  // flags
  dataLoadingFlag: boolean = true;
  userDataLoadingFlag: boolean = true;



  constructor (private userService: UserService) { 
    
  }

  async ngOnInit(): Promise<void> {

    if (!this.userService.currentUserObject) {
      console.log("This user object is undefined");
      await this.userService.setCurrentUserSubject();
    }
 
    this.userService.currentUserObject.subscribe(userObj => {  this.userObj = userObj; } )

    this.dataLoadingFlag = false;
    
  }


}
