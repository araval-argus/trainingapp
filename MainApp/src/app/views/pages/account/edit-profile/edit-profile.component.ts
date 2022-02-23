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

  hostUrl: string = environment.hostUrl;

  user: LoggedInUser;
  editForm: FormGroup;

  // flags
  dataLoadingFlag: boolean = true;

  // utils 
  uploadProgress: number = 0;
  uploadSubscription: Subscription;
  userProfileImageLink:string = "/assets/images/placeholder.jpg";
  timeStamp: Date;

  constructor(
    private userService: UserService
  ) { 

    this.editForm = new FormGroup({
      firstName: new FormControl('', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]),
      lastName: new FormControl('', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]),
      userName: new FormControl('',  [Validators.required, Validators.minLength(2), Validators.maxLength(50)]),
      email: new FormControl('', [Validators.required, Validators.email])
    })

    this.fetchProfileUrl() 
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


  public uploadFile = (files) => {
    if (files === 0) {
      return;
    }


    let fileToUpload = <File>files[0]
    const formData = new FormData();
    formData.append('profileImage', fileToUpload);

    this.uploadProgress = 0;

    this.uploadSubscription =  this.userService.updateUserProfileImage(formData).subscribe(
      (res) => {
        if (res.type == HttpEventType.UploadProgress) {
          this.uploadProgress = Math.round(100 * (res.loaded / res.total));
        }
        console.log("This is the response: ");
        // this.userProfile = ;
        console.log(res);

        if (res.type == 4) {
          this.userProfileImageLink = this.hostUrl + "/" + res.body.filePath;
          this.updateProfileImage()
        }
        
      },
      (err) => {
        console.log(err);
        
      }, 
      () => {
        Swal.fire({
          title: 'Success!',
          text: 'Profile uploaded successfully.',
          icon: 'success',
          timer: 2000,
         timerProgressBar: true,
        });
        console.log("Request completed");

        
        this.uploadProgress = 0;

      }
    )
  }

  cancelUpload() {
    this.uploadSubscription.unsubscribe();
    this.reset();
  }

  reset() {
    this.uploadProgress = 0;
    this.uploadSubscription = null
  }

  public getLinkPicture() {
    if(this.timeStamp) {
       return this.userProfileImageLink + '?' + this.timeStamp;
    }
    return this.userProfileImageLink;
  }

  fetchProfileUrl() {
    this.userService.getUserProfileUrl().subscribe(
      (res) => {
        if (res.profileUrl == "") {
          return;
        }

        this.userProfileImageLink = this.hostUrl + "/" + res.profileUrl;
        this.updateProfileImage();
      },
      (err) => {
        console.log(err);
      }
    )
  }

  public updateProfileImage() {
    
    this.timeStamp = new Date();
    // this.userProfileImageLink + '?' + this.timeStamp;
}

}
