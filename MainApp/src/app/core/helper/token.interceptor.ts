import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../service/auth-service';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  token:any;

  constructor(
    private authAervice: AuthService
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.token = this.authAervice.getUserToken()

    if (this.token) {
      const authReq = request.clone({
        headers: request.headers.set('Authorization', "bearer "+ this.token)
      })
      return next.handle(authReq)
    }

    return next.handle(request);
  }
}
