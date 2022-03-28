import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { UserService } from 'src/app/core/service/user.service';
import { environment } from 'src/environments/environment';
import Swal from 'sweetalert2';

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
      email: new FormControl('', [Validators.required, Validators.email]),
      statusText: new FormControl('', [Validators.required, Validators.minLength(5)])
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
      'email': this.userObj.email,
      'statusText': this.userObj.statusText
    })
  }

  onUpdate() {
    let userUpdateObj = {
      'firstName': this.editForm.value.firstName,
      'lastName': this.editForm.value.lastName,
      'userName': this.editForm.value.userName,
      'email': this.editForm.value.email,
      'statusText': this.editForm.value.statusText
    }
    
    console.log("Updating values");
    console.log(userUpdateObj);
    
    
    this.userService.updateUserDetails(userUpdateObj).subscribe(data => {
      let updateObj = data;

      if (updateObj.profileUrl) {
        updateObj.profileUrl = environment.hostUrl + "/" + data.profileUrl;
      }
      this.userService.updateUserObject(updateObj)
      console.log(data);

      
      Swal.fire({
        title: 'Success!',
        text: 'Details Updated successfully.',
        icon: 'success',
        timer: 2000,
        timerProgressBar: true,
      });
      console.log("Request completed");
      
    },
    (err) => {
      console.log(err);
      
    })
  }

}
