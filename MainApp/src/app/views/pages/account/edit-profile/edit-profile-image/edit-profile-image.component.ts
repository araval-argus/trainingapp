import { HttpEventType } from '@angular/common/http';
import { Component, Input, OnInit } from '@angular/core';
import { Subscription } from 'rxjs/internal/Subscription';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { UserService } from 'src/app/core/service/user.service';
import { environment } from 'src/environments/environment';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-edit-profile-image',
  templateUrl: './edit-profile-image.component.html',
  styleUrls: ['./edit-profile-image.component.scss']
})
export class EditProfileImageComponent implements OnInit {

  @Input() userObj: LoggedInUser;
   
  // utils 
  uploadProgress: number = 0;
  uploadSubscription: Subscription;
  userProfileImageLink:string;
  timeStamp: Date = new Date();

  constructor(private userService: UserService) { }

  ngOnInit(): void {
  }
  
  uploadFile(files){
    if (files === 0) { return; }

    let fileToUpload = <File>files[0]
    const formData = new FormData();
    formData.append('profileImage', fileToUpload);

    this.uploadProgress = 0;

    this.uploadSubscription =  this.userService.updateUserProfileImage(formData).subscribe(
      (res) => {
        if (res.type == HttpEventType.UploadProgress) {
          this.uploadProgress = Math.round(100 * (res.loaded / res.total));
        }
        if (res.type == HttpEventType.Response) {
          this.userProfileImageLink = environment.hostUrl + "/" + res.body.filePath;
          this.updateProfileImage()

          this.userObj.profileUrl = this.userProfileImageLink + '?' + new Date();
          this.userService.updateUserObject(this.userObj)
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

  getLinkPicture() { return this.userProfileImageLink + '?' + this.timeStamp; }


  public updateProfileImage() { this.timeStamp = new Date(); }

}
