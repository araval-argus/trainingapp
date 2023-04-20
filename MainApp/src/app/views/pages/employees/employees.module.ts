import { NgModule } from "@angular/core";
import { RouterModule, Routes } from '@angular/router';
import { EmployeesComponent } from './employees.component';
import { TableModule } from 'primeng/table'
import { ButtonModule } from 'primeng/button';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

const routes: Routes = [
  {
    path : '',
    component: EmployeesComponent
  }
]
@NgModule({
  declarations:[
    EmployeesComponent
  ],
  imports: [
    RouterModule.forChild(routes),
    TableModule,
    ButtonModule,
    FormsModule,
    CommonModule
  ]
})
export class EmployeesModule{}
