import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../service/auth-service';

@Injectable({
  providedIn: 'root'
})
export class EmployeesAuthGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router){
  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      const designation = this.authService.getLoggedInUserInfo().designation
      if(designation !== "Intern" && designation !== "Probationer"){
        return true;
      }
      console.log("navigate to dashboard")
      this.router.navigate([''])
      return false;
  }

}
