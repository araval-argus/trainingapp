import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { UserService } from 'src/app/core/service/user.service';

@Component({
  selector: 'app-edit-profile-details',
  templateUrl: './edit-profile-details.component.html',
  styleUrls: ['./edit-profile-details.component.scss']
})
export class EditProfileDetailsComponent implements OnInit {
  
  @Input() userObj:LoggedInUser;
  
  editForm: FormGroup;

  constructor(private userService: UserService) { 
    this.editForm = new FormGroup({
      firstName: new FormControl('', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]),
      lastName: new FormControl('', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]),
      userName: new FormControl('',  [Validators.required, Validators.minLength(2), Validators.maxLength(50)]),
      email: new FormControl('', [Validators.required, Validators.email])
    })

  }
  
  ngOnInit(): void {
    this.setFormValues()
  }

  private setFormValues() {
    this.editForm.setValue({
      'firstName': this.userObj.firstName,
      'lastName': this.userObj.lastName,
      'userName': this.userObj.userName,
      'email': this.userObj.email
    })
  }

  onUpdate() {
    let userUpdateObj = {
      'firstName': this.editForm.value.firstName,
      'lastName': this.editForm.value.lastName,
      'userName': this.editForm.value.userName,
      'email': this.editForm.value.email
    }
    this.userService.updateUserDetails(userUpdateObj).subscribe(data => {
      console.log(data);
      this.userService.updateUserObject(data)
      
    },
    (err) => {
      console.log(err);
      
    })
  }

}
