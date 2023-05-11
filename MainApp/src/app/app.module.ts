import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HIGHLIGHT_OPTIONS } from 'ngx-highlightjs';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';

import { AppRoutingModule } from './app-routing.module';
import { LayoutModule } from './views/layout/layout.module';
import { AuthGuard } from './core/guard/auth.guard';
import { AppComponent } from './app.component';
import { ErrorPageComponent } from './views/pages/error-page/error-page.component';
import { TokenInterceptor } from './core/helper/token-interceptor';
import { AuthService } from './core/service/auth-service';
import { EditFormComponent } from './views/pages/edit-form/edit-form.component';
import { ModalComponent } from './views/pages/ui-components/modal/modal.component';
import { ChartsModule } from 'ng2-charts';


@NgModule({
  declarations: [
    AppComponent,
    ErrorPageComponent,
    EditFormComponent,
    ModalComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    LayoutModule,
    HttpClientModule,
    FormsModule,
    ButtonModule,
    ChartsModule
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
