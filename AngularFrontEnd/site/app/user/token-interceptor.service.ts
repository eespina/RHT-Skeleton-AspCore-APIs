import { Injectable, Injector } from '@angular/core';
import { HttpInterceptor } from '@angular/common/http'
import { AuthService } from './auth.service';

@Injectable()
//This helps ensure that the PROPER token that was created from the server-side is used in further requests when kept LOGGED IN.
export class TokenInterceptorService implements HttpInterceptor {

    constructor(private injector: Injector) { }

    //Intercepts outgoing http requests. transforms them, THEN sends them to the server for processing.
    ////It modifies the request to contain the token stored in teh browsers local storage.
    intercept(req, next) {
        let authService = this.injector.get(AuthService);
        let tokenizedRequest = req.clone(
            {
                headers: req.headers.set('Authorization', 'bearer ' + authService.getToken())
                    //.set('Content-Type', 'application/json')    //'application/x-www-form-urlencoded;charset=utf-8'
            }
        );

        return next.handle(tokenizedRequest);
    }
}