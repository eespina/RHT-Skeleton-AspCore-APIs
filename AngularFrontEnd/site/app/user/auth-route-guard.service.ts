import { Injectable } from '@angular/core';
import { IUser } from '../user/user';
import { ActivatedRouteSnapshot, RouterStateSnapshot, Router, Route, CanActivate, CanActivateChild, CanLoad } from '@angular/router';
import { AuthService } from './auth.service';
import { RegisterComponent } from './register.component';

@Injectable()
export class AuthGuard implements CanActivate, CanActivateChild, CanLoad {

    constructor(private authService: AuthService, private router: Router) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        console.log('In canActivate: ' + state.url);
        let isLoggedInAlready = this.checkLoggedIn(state.url);
        return isLoggedInAlready;
    }

    canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        console.log('In canActivateChild: ' + state.url);
        return this.checkLoggedIn(state.url);
    }

    canLoad(route: Route): boolean {
        console.log('In canLoad: ' + route.path);
        return this.checkLoggedIn(route.path);
    }

    checkLoggedIn(url: string): boolean {
        if (this.authService.loggedIn()) {
            return true;
        }

        // Retain the attempted URL for redirection
        this.authService.redirectUrl = url;
        this.router.navigate(['/login']);
        return false;
    }

    //being coupled with the RegisterComponent, I can no longer use this for the "ReactiveFormComponent"
        //therefore, I cannot check the 'dirty' form material, if it exists
    canDeactivate(component: RegisterComponent): boolean {
        console.log('In canDeactivate: ' + RegisterComponent.name);
        if (component.createExampleForm.dirty) {
            return confirm('Are you sure you want to \'discard\' your changes ?');
        }
        return true;
    }
}
