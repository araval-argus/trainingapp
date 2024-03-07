import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GeneralComponent } from './general.component';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { NgSelectModule } from '@ng-select/ng-select';

const routes: Routes = [
  {
    path: "",
    component: GeneralComponent,
    children:[],
  },
];

@NgModule({
  declarations: [
    GeneralComponent
  ],
  imports: [
    RouterModule.forChild(routes),
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PerfectScrollbarModule,
    NgSelectModule,
  ],  
  exports: [RouterModule],
})
export class GeneralModule { }
