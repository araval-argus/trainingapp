import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChatComponent } from './chat.component';
import { SearchDropdownComponent } from './chat-sidebar/search-dropdown/search-dropdown.component';
import { MessageComponent } from './message/message.component';
import { ChatSidebarComponent } from './chat-sidebar/chat-sidebar.component';
import { RecentChatComponent } from './chat-sidebar/recent-chat/recent-chat.component';
import { ReplyMessageComponent } from './reply-message/reply-message.component';
import { FileModalComponent } from './file-modal/file-modal.component';
import { PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
import { RouterModule } from '@angular/router';
import { NgbCollapseModule, NgbDropdownModule, NgbNavModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true,
  swipeEasing: true
};

const routes = [
  {
    path: '',
    component: ChatComponent
  }

]

@NgModule({
  declarations: [ChatComponent, SearchDropdownComponent, MessageComponent, ChatSidebarComponent, RecentChatComponent, ReplyMessageComponent, FileModalComponent],
  imports: [
    CommonModule,
    PerfectScrollbarModule,
    RouterModule.forChild(routes),
    NgbTooltipModule,
    PerfectScrollbarModule,
    NgbDropdownModule,
    NgbNavModule,
    NgbCollapseModule,
    NgSelectModule,
  ],
  providers: [
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    }
  ]
})
export class ChatModule { }
