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

////TODO - Think about creating an ERROR Interceptor (below, but in it's own .ts file)
//import { Injectable } from '@angular/core';
//import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
//import { Observable } from 'rxjs/Rx';
//import { catchError } from 'rxjs/operators';

//import { AuthenticationService } from '../Services/authentication.service';
//import { Router } from '@angular/router';

//@Injectable()
//export class ErrorInterceptor implements HttpInterceptor {
//    constructor(private authenticationService: AuthenticationService, private router: Router) { }

//    intercept(request: HttpRequest<any>, newRequest: HttpHandler): Observable<HttpEvent<any>> {

//        return newRequest.handle(request).pipe(catchError(err => {
//            if (err.status === 401) {
//                //if 401 response returned from api, logout from application & redirect to login page.
//                this.authenticationService.logout();
//            }

//            const error = err.error.message || err.statusText;
//            return Observable.throw(error);
//        }));
//    }
//}