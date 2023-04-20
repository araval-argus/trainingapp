import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GroupMessageComponent } from './group-message/group-message.component';
import { GroupSidebarComponent } from './group-sidebar/group-sidebar.component';
import { GroupComponent } from './group.component';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { NgbDropdownModule, NgbTooltip, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarConfigInterface, PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PickerModule } from '@ctrl/ngx-emoji-mart';

const routes = [
  {
    path: '',
    component: GroupComponent
  }
]

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true,
  swipeEasing: true
};

@NgModule({
  declarations: [GroupMessageComponent, GroupSidebarComponent, GroupComponent],
  imports: [
    RouterModule.forChild(routes),
    CommonModule,
    ReactiveFormsModule,
    CommonModule,
    FormsModule,
    NgSelectModule,
    NgbDropdownModule,
    PerfectScrollbarModule,
    PickerModule,
    NgbTooltipModule
  ],
  providers: [
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    }
  ]
})
export class GroupModule { }
