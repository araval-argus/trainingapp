import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserProfileComponent } from './user-profile/user-profile.component';
import { RouterModule, Routes } from '@angular/router';
import { EditProfileComponent } from './edit-profile/edit-profile.component';
import { AccountComponent } from './account.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { FeahterIconModule } from 'src/app/core/feather-icon/feather-icon.module';
import { EditProfileImageComponent } from './edit-profile/edit-profile-image/edit-profile-image.component';
import { EditProfileDetailsComponent } from './edit-profile/edit-profile-details/edit-profile-details.component';

const routes: Routes = [
  {
    path: '',
    component: AccountComponent,

    children: [
      {
        path: '',
        redirectTo: 'profile',
        pathMatch: 'full'
      },
      {
        path: 'profile',
        component: UserProfileComponent
      },
      {
        path: 'edit',
        component: EditProfileComponent
      }
    ]
  }
]

@NgModule({
  declarations: [UserProfileComponent, EditProfileComponent, AccountComponent, EditProfileImageComponent, EditProfileDetailsComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    FormsModule,
    ReactiveFormsModule,
    FeahterIconModule,
    NgbModule
  ]
})
export class AccountModule { }
