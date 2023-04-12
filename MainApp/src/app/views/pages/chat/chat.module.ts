import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { NgbDropdownModule, NgbTooltipModule, NgbNavModule, NgbCollapseModule } from '@ng-bootstrap/ng-bootstrap';

import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { ChatComponent } from './chat.component';
import { ChatSidebarComponent } from './chat-sidebar/chat-sidebar.component';
import { ChatLoadComponent } from './chat-load/chat-load.component';
import { ScrollToBottomDirective } from 'src/app/core/helper/scroll-to-bottom.directive';

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
  declarations: [ChatComponent , ChatSidebarComponent , ChatLoadComponent, ScrollToBottomDirective],
  imports: [
    RouterModule.forChild(routes),
    NgbDropdownModule,
    NgbTooltipModule,
    NgbNavModule,
    NgbCollapseModule,
    PerfectScrollbarModule,
  ],
  providers: [
  ]
})
export class ChatModule { }
