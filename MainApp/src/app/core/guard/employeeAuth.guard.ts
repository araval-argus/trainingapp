import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from "@angular/router";
import { AuthService } from "../../core/service/auth-service"

@Injectable({
    providedIn: 'root'
})
export class EmployeeAuthGuard implements CanActivate {
    constructor(private router: Router, private authService: AuthService) { }

    canActivate() {
        const desingnation = this.authService.getLoggedInUserInfo().desingnation;
        if (desingnation != "Intern" && desingnation != "Probationer") {
            return true;
        }

        this.router.navigate(['']);
        return false;
    }
}
