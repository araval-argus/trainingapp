import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';

import { LayoutModule } from './views/layout/layout.module';
import { AuthGuard } from './core/guard/auth.guard';

import { AppComponent } from './app.component';
import { ErrorPageComponent } from './views/pages/error-page/error-page.component';

import { HIGHLIGHT_OPTIONS } from 'ngx-highlightjs';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TokenInterceptor } from './core/helper/token-interceptor';
import { AuthService } from './core/service/auth-service';
import { EditFormComponent } from './views/pages/edit-form/edit-form.component';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { NgbCollapseModule, NgbDropdownModule, NgbNavModule, NgbTooltipModule, NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import { ChatComponent } from './views/pages/chat/chat.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { ChatSidebarComponent } from './views/pages/chat/chat-sidebar/chat-sidebar.component';

import { DropdownDirective } from './core/helper/dropdown-directive';
import { FriendsDropdownComponent } from './views/pages/chat/chat-sidebar/friends-dropdown/friends-dropdown.component';
import { ChatContentComponent } from './views/pages/chat/chat-content/chat-content.component';
import { ChatContentBodyComponent } from './views/pages/chat/chat-content/chat-content-body/chat-content-body.component';
import { ChatContentFooterComponent } from './views/pages/chat/chat-content/chat-content-footer/chat-content-footer.component';
import { ChatContentHeaderComponent } from './views/pages/chat/chat-content/chat-content-header/chat-content-header.component';

import { ModalComponent } from 'src/app/views/pages/ui-components/modal/modal.component';


@NgModule({
  declarations: [
    AppComponent,
    ErrorPageComponent,
    EditFormComponent,
    ChatComponent,
    ChatSidebarComponent,
    ChatContentComponent,
    DropdownDirective,
    FriendsDropdownComponent,
    ChatContentHeaderComponent,
    ChatContentBodyComponent,
    ChatContentFooterComponent,
    ModalComponent
  ],
  schemas:[CUSTOM_ELEMENTS_SCHEMA],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    LayoutModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    TableModule,
    ButtonModule,
    NgbDropdownModule,
    NgbTooltipModule,
    NgbNavModule,
    NgbCollapseModule,
    NgSelectModule,
    PerfectScrollbarModule
  ],
  providers: [
    AuthGuard,
    {
      provide: HIGHLIGHT_OPTIONS, // https://www.npmjs.com/package/ngx-highlightjs
      useValue: {
        coreLibraryLoader: () => import('highlight.js/lib/core'),
        languages: {
          xml: () => import('highlight.js/lib/languages/xml'),
          typescript: () => import('highlight.js/lib/languages/typescript'),
          scss: () => import('highlight.js/lib/languages/scss'),
        }
      }
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: TokenInterceptor,
      multi: true,
      deps: [AuthService],
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
