import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { GroupsComponent } from "./groups.component";
import { GroupsSidebarComponent } from "./groups-sidebar/groups-sidebar.component";
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { FormsModule } from '@angular/forms';
import { GroupContentComponent } from './group-content/group-content.component';
import { GroupContentHeaderComponent } from './group-content/group-content-header/group-content-header.component';
import { GroupContentBodyComponent } from './group-content/group-content-body/group-content-body.component';
import { NgbDropdownModule,  NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { PickerModule } from '@ctrl/ngx-emoji-mart';

const routes: Routes = [
  {
    path: "",
    component: GroupsComponent,
  },
];
@NgModule({
  declarations: [
    GroupsComponent,
    GroupsSidebarComponent,
    GroupContentComponent,
    GroupContentHeaderComponent,
    GroupContentBodyComponent
  ],
  imports: [
    RouterModule.forChild(routes),
    PerfectScrollbarModule,
    FormsModule,
    NgbDropdownModule,
    NgbTooltipModule,
    PickerModule
  ]
})
export class GroupModule {}
