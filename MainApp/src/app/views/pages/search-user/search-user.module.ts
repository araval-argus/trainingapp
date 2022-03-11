import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { SearchUserComponent } from './search-user.component';
import { FeatherIconModule } from 'src/app/core/feather-icon/feather-icon.module';

const routes: Routes = [
  {
    path: '',
    component: SearchUserComponent
  }
]

@NgModule({
  declarations: [SearchUserComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    FeatherIconModule
  ]
})
export class SearchUserModule { }
