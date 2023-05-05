import { CommonModule } from '@angular/common';
import { NgModule } from "@angular/core";
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { NgbDropdownModule, NgbNavModule,  NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { FeahterIconModule } from 'src/app/core/feather-icon/feather-icon.module';
import { DropdownDirective } from 'src/app/core/helper/dropdown-directive';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { PickerModule } from '@ctrl/ngx-emoji-mart';

import { ChatContentBodyComponent } from './chat-content/chat-content-body/chat-content-body.component';
import { ChatContentFooterComponent } from './chat-content/chat-content-footer/chat-content-footer.component';
import { ChatContentHeaderComponent } from './chat-content/chat-content-header/chat-content-header.component';
import { ChatContentComponent } from './chat-content/chat-content.component';
import { ChatSidebarComponent } from './chat-sidebar/chat-sidebar.component';
import { FriendsDropdownComponent } from './chat-sidebar/friends-dropdown/friends-dropdown.component';
import { ChatComponent } from './chat.component';


const routes: Routes = [
  {
    path: '',
    component: ChatComponent
  }
]


@NgModule({
  declarations:[
    ChatComponent,
    ChatSidebarComponent,
    ChatContentComponent,
    DropdownDirective,
    FriendsDropdownComponent,
    ChatContentHeaderComponent,
    ChatContentBodyComponent,
    ChatContentFooterComponent
  ],
  imports:[
    RouterModule.forChild(routes),
    CommonModule,
    FormsModule,
    FeahterIconModule,
    NgbDropdownModule,
    NgbTooltipModule,
    PerfectScrollbarModule,
    NgbNavModule,
    PickerModule
  ],
  exports:[]
})
export class ChatModule{}
