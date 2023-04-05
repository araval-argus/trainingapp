import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggedInUser } from 'src/app/core/models/loggedin-user';
import { AccountService } from 'src/app/core/service/account-service';
import { AuthService } from 'src/app/core/service/auth-service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-edit-profile',
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.scss']
})
export class EditProfileComponent implements OnInit {
  // To get current Details
  loggedInUser: LoggedInUser;
  // To get UserInput
  updatedDetails: LoggedInUser = {
    firstName: '',
    lastName: '',
  };
  // To accept from html
  editForm: FormGroup;
  returnUrl: any;
  profileImage: string = null;
  formData: FormData = new FormData();
  constructor(private route: ActivatedRoute, private authService: AuthService, private router: Router, private accountService: AccountService) { }

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.editForm = new FormGroup({
      firstName: new FormControl(this.loggedInUser.firstName, Validators.required),
      lastName: new FormControl(this.loggedInUser.lastName, Validators.required),
      email: new FormControl(this.loggedInUser.email, [Validators.required, Validators.email]),
    })
    this.profileImage = localStorage.getItem('imagePath');
  }

  onSubmit($event) {
    const file: File = event.target[4].files[0];
    if (file != null) {
      this.formData.append('file', file, file.name);
      // localStorage.setItem('imagePath', "https:\\localhost:44382\\Profile_Images\\" + this.loggedInUser.sub + "_Profile_Image.jpg");
    }
    this.formData.append('firstName', this.editForm.value.firstName);
    this.formData.append('lastName', this.editForm.value.lastName);
    this.formData.append('email', this.editForm.value.email);
    this.accountService.update(this.formData, this.loggedInUser.sub).subscribe((data: any) => {
      localStorage.setItem('USERTOKEN', data.token);
      this.accountService.getImage(this.loggedInUser.sub).subscribe((data: any) => {
        this.loggedInUser.imageURL = data.imgPath;
        localStorage.setItem('imagePath', this.accountService.getImageUrl(data.imgPath));
        this.accountService.updateDetails.next();
      })
      Swal.fire({
        title: 'Success!',
        text: 'Details Updated Successfully.',
        icon: 'success',
        timer: 2000,
        timerProgressBar: true,
      });
      setTimeout(() => {
        this.router.navigate(["/"]);
      }, (3000));
      this.router.navigate([this.returnUrl]);
    },
      (error) => {
        console.log(error);
        Swal.fire({
          title: 'Something went wrong!',
          text: error.error.message,
          icon: 'error',
          timer: 2000,
          timerProgressBar: true,
        });
      }
    )
  }

  onCancel() {
    this.router.navigate(['/']);
  }

}
