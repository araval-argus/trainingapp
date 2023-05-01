import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
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
  uploadedFile: File;
  formData: FormData = new FormData();
  unsafeUrl: any;
  url: any;
  changePasswordForm: FormGroup;


  //To toggle eye button
  toggleOldPwdEye: boolean = false;
  toggleNewPwdEye: boolean = false;
  toggleNewCnfPwdEye: boolean = false;
  constructor(private route: ActivatedRoute, private authService: AuthService, private router: Router, private accountService: AccountService, private domSanitizer: DomSanitizer, private modalService: NgbModal) { }

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    this.loggedInUser = this.authService.getLoggedInUserInfo();
    this.editForm = new FormGroup({
      firstName: new FormControl(this.loggedInUser.firstName, Validators.required),
      lastName: new FormControl(this.loggedInUser.lastName, Validators.required),
      email: new FormControl(this.loggedInUser.email, [Validators.required, Validators.email]),
    })
    this.profileImage = localStorage.getItem('imagePath');

    this.changePasswordForm = new FormGroup({
      currentPassword: new FormControl('', Validators.required),
      newPassword: new FormControl('', Validators.required),
      confirmPassword: new FormControl('', Validators.required),
    })
  }

  onSubmit($event) {
    this.formData.append('file', this.uploadedFile);
    this.formData.append('firstName', this.editForm.value.firstName);
    this.formData.append('lastName', this.editForm.value.lastName);
    this.formData.append('email', this.editForm.value.email);
    this.accountService.update(this.formData, this.loggedInUser.sub).subscribe((data: any) => {
      localStorage.setItem('USERTOKEN', data.token);
      this.accountService.getImage(this.loggedInUser.sub).subscribe((data: any) => {
        this.loggedInUser.imageURL = data.imgPath;
        localStorage.setItem('imagePath', this.accountService.fetchImage(data.imgPath));
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
    this.url = null;
  }


  fileUploaded(event) {
    if (event.target.files.length > 0) {
      this.uploadedFile = event.target.files[0];
      var reader = new FileReader();
      reader.readAsDataURL(this.uploadedFile);
      reader.onload = (event) => {
        this.unsafeUrl = (<FileReader>event.target).result;
        this.url = this.domSanitizer.bypassSecurityTrustResourceUrl(this.unsafeUrl);
      }
    }
  }

  //will open the modal for changing password
  openModal(content: TemplateRef<any>) {
    console.log("Called");

    this.modalService.open(content, { backdrop: false, keyboard: false, centered: true })
  }


  //to reset modal
  clearPwdForm() {
    this.changePasswordForm.reset();
    this.toggleOldPwdEye = false;
    this.toggleNewPwdEye = false;
    this.toggleNewCnfPwdEye = false;
  }



  changePassword() {
    const changePwdFormData: FormData = new FormData();
    changePwdFormData.append('OldPassword', this.changePasswordForm.value.currentPassword);
    changePwdFormData.append('NewPassword', this.changePasswordForm.value.confirmPassword);
    this.accountService.changePassword(changePwdFormData).subscribe((data) => {
      if (data) {
        Swal.fire({
          title: 'Success!',
          text: 'Password Updated Successfully.',
          icon: 'success',
          timer: 2000,
          timerProgressBar: true,
        });
      } else {
        Swal.fire({
          title: 'Fail!',
          text: 'Old Password Did Not Match.',
          icon: 'error',
          timer: 2000,
          timerProgressBar: true,
        });
      }

    })
    this.clearPwdForm();
  }
}
