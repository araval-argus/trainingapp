import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../service/auth-service';
import { LoggedInUser } from '../models/loggedin-user';

@Injectable({
  providedIn: 'root'
})
export class EmployeeGuard implements CanActivate {

  constructor(private authService:AuthService , private router:Router){}
  loggedInUser : LoggedInUser;

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      this.loggedInUser = this.authService.getLoggedInUserInfo();
      if(this.loggedInUser.designation!='Probationer'&&this.loggedInUser.designation!='Intern')
      {
        return true;
      }
      else{
        this.router.navigate(['']);
        return false;
      }
  }
}
