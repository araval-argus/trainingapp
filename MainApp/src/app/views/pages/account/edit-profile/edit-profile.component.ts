import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { UserService } from 'src/app/core/service/user.service';

@Component({
  selector: 'app-edit-profile',
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.scss']
})
export class EditProfileComponent implements OnInit {

  user: LoggedInUser;
  editForm: FormGroup;

  // flags
  dataLoadingFlag: boolean = true;

  constructor(
    private userService: UserService
  ) { 

    this.editForm = new FormGroup({
      firstName: new FormControl('', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]),
      lastName: new FormControl('', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]),
      userName: new FormControl('',  [Validators.required, Validators.minLength(2), Validators.maxLength(50)]),
      email: new FormControl('', [Validators.required, Validators.email])
    })
  }

  ngOnInit(): void {
    this.fetchUserDetails();
  }

  private fetchUserDetails() {
    this.userService.getCurrentUserDetails().subscribe((result) => {
      this.user = result;
      console.log(result);
      this.dataLoadingFlag = false;
      this.setFormValues();
    },
    (err) => {
      console.log(err);
      this.dataLoadingFlag = false;
    });
  }


  private setFormValues() {
    this.editForm.setValue({
      'firstName': this.user.firstName,
      'lastName': this.user.lastName,
      'userName': this.user.userName,
      'email': this.user.email
    })
  }

  onUpdate() {
    let userObj = {
      'firstName': this.editForm.value.firstName,
      'lastName': this.editForm.value.lastName,
      'userName': this.editForm.value.userName,
      'email': this.editForm.value.email
    }
    this.userService.updateUserDetails(userObj).subscribe(data => {
      console.log(data);
      
    },
    (err) => {
      console.log(err);
      
    })
  }

}
