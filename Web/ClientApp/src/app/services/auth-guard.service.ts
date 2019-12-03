import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot, CanActivateChild, NavigationExtras, CanLoad, Route } from '@angular/router';
import * as generated from './endpoint.services';


@Injectable()
export class AuthGuardService implements CanActivate, CanActivateChild, CanLoad {
    constructor(private authEndpointService: generated.AuthEndpointService, private router: Router) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {

        const url: string = state.url;
        return this.checkLogin(url);
    }

    canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        return this.canActivate(route, state);
    }

    canLoad(route: Route): boolean {

        const url = `/${route.path}`;
        return this.checkLogin(url);
    }

    checkLogin(url: string): boolean {

        if (this.authEndpointService.isLoggedIn) {
            return true;
        }

        this.authEndpointService.loginRedirectUrl = url;
        this.router.navigate(['/login']);

        return false;
    }
}
