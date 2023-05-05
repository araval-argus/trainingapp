import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmployeeComponent } from './employee.component';
import { TableModule } from 'primeng/table'
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RadioButtonModule } from 'primeng/radiobutton';
import { SelectButtonModule } from 'primeng/selectbutton';
import { ToastModule } from 'primeng/toast';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { EmployeeAuthGuard } from 'src/app/core/guard/employeeAuth.guard';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { DialogModule } from 'primeng/dialog';
import { NgSelectModule } from '@ng-select/ng-select';
import { ConfirmationService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';




const routes = [
  {
    path: '',
    component: EmployeeComponent,
    canActivate: [EmployeeAuthGuard],
  }

]

@NgModule({
  declarations: [EmployeeComponent],
  imports: [
    RouterModule.forChild(routes),
    CommonModule,
    TableModule,
    FormsModule,
    ReactiveFormsModule,
    RadioButtonModule,
    SelectButtonModule,
    ToastModule,
    DropdownModule,
    InputTextModule,
    ButtonModule,
    RippleModule,
    PerfectScrollbarModule,
    DialogModule,
    NgSelectModule,
    ConfirmDialogModule,
    FormsModule
  ],
  providers: [ConfirmationService]
})
export class EmployeeModule { }
