import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EditProfileComponent } from './edit-profile/edit-profile.component';
import { ViewProfileComponent } from './view-profile/view-profile.component';
import { RouterModule } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';


const routes = [
  {
    path: 'viewProfile',
    component: ViewProfileComponent
  },
  {
    path: 'editProfile',
    component: EditProfileComponent
  }
]


@NgModule({
  declarations: [EditProfileComponent, ViewProfileComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    ReactiveFormsModule
  ]
})
export class ProfileModule { }
