import { RouterModule, Routes } from "@angular/router";
import { GroupComponent } from "./group.component";
import { GroupLoadComponent } from "./group-load/group-load.component";
import { NgModule } from "@angular/core";
import { GroupSidebarComponent } from "./group-sidebar/group-sidebar.component";
import { NgbCollapseModule, NgbDropdownModule, NgbNavModule, NgbTooltipModule } from "@ng-bootstrap/ng-bootstrap";
import { PerfectScrollbarModule } from "ngx-perfect-scrollbar";
import { PickerModule } from "@ctrl/ngx-emoji-mart";
import { FormsModule } from "@angular/forms";
import { NgSelectModule } from "@ng-select/ng-select";

const routes: Routes = [
  {
    path: '',
    component: GroupComponent,
    children: [
      {
        path:':grpId' , component:GroupLoadComponent
      }
    ]
  }
]

@NgModule({
  declarations: [GroupComponent , GroupSidebarComponent , GroupLoadComponent ],
  imports: [
    RouterModule.forChild(routes),
    NgbDropdownModule,
    NgbTooltipModule,
    FormsModule,
    NgSelectModule,
    NgbNavModule,
    NgbCollapseModule,
    PerfectScrollbarModule,
    PickerModule
  ],
  providers: [
  ],
})
export class GroupModule { }
