import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { SearchUserComponent } from './search-user.component';

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
    RouterModule.forChild(routes)
  ]
})
export class SearchUserModule { }
