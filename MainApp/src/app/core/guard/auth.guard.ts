import { Injectable } from '@angular/core';
import { CanActivate, RouterStateSnapshot, ActivatedRouteSnapshot } from '@angular/router';
import { Router } from '@angular/router';
import { JwtHelper } from '../helper/jwt-helper';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(private router: Router, private jwtHelper: JwtHelper) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (localStorage.getItem('isLoggedin')) {

      // logged in so return true
      const exp = this.jwtHelper.decodeToken(localStorage.getItem('USERTOKEN')).exp;
      if (Date.now() / 1000 <= exp) {
        return true;
      }
    }

    // not logged in so redirect to login page with the return url
    this.router.navigate(['/auth/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }
}