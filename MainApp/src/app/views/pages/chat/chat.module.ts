import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChatComponent } from './chat.component';
import { ChatSidebarComponent } from './chat-sidebar/chat-sidebar.component';
import { ChatPageComponent } from './chat-page/chat-page.component';
import { RouterModule, Routes } from '@angular/router';
import { FeatherIconModule } from 'src/app/core/feather-icon/feather-icon.module';
import { NgbModule, NgbNav, NgbNavModule } from '@ng-bootstrap/ng-bootstrap';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

const routes: Routes = [
  {
    path: ':user',
    component: ChatComponent,
  },
  {
    path: '',
    component: ChatComponent,
  }
]

@NgModule({
  declarations: [ChatComponent, ChatSidebarComponent, ChatPageComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    FeatherIconModule,
    NgbModule,
    NgbNavModule,
    PerfectScrollbarModule,
    ReactiveFormsModule,
    FormsModule
  ]
})
export class ChatModule { }
