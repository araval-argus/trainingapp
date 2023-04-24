import { NgModule  } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { NgbDropdownModule, NgbTooltipModule, NgbNavModule, NgbCollapseModule } from '@ng-bootstrap/ng-bootstrap';

import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { ChatComponent } from './chat.component';
import { ChatSidebarComponent } from './chat-sidebar/chat-sidebar.component';
import { ChatLoadComponent } from './chat-load/chat-load.component';
import { ClickOutsideDirective } from 'src/app/core/helper/click-outside.directive';
import { PickerModule } from '@ctrl/ngx-emoji-mart';

const routes: Routes = [
  {
    path: '',
    component: ChatComponent,
    children: [
      {
        path:':username' , component:ChatLoadComponent
      }
    ]
  }
]

@NgModule({
  declarations: [ChatComponent , ChatSidebarComponent , ChatLoadComponent, ClickOutsideDirective ],
  imports: [
    RouterModule.forChild(routes),
    NgbDropdownModule,
    NgbTooltipModule,
    NgbNavModule,
    NgbCollapseModule,
    PerfectScrollbarModule,
    PickerModule
  ],
  providers: [
  ],
})
export class ChatModule { }
