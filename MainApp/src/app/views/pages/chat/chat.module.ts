import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChatComponent } from './chat.component';
import { SearchDropdownComponent } from './chat-sidebar/search-dropdown/search-dropdown.component';
import { MessageComponent } from './message/message.component';
import { ChatSidebarComponent } from './chat-sidebar/chat-sidebar.component';
import { RecentChatComponent } from './chat-sidebar/recent-chat/recent-chat.component';
import { ReplyMessageComponent } from './message/reply-message/reply-message.component';
import { PERFECT_SCROLLBAR_CONFIG, PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
import { RouterModule } from '@angular/router';
import { NgbCollapseModule, NgbDropdownModule, NgbNavModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { NgSelectModule } from '@ng-select/ng-select';
import { PickerModule } from '@ctrl/ngx-emoji-mart';




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
  declarations: [ChatComponent, SearchDropdownComponent, MessageComponent, ChatSidebarComponent, RecentChatComponent, ReplyMessageComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    NgbTooltipModule,
    PerfectScrollbarModule,
    NgbDropdownModule,
    NgbNavModule,
    NgbCollapseModule,
    NgSelectModule,
    PickerModule
  ],
  providers: [
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    }
  ]
})
export class ChatModule { }
