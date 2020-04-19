import { Injectable } from '@angular/core';
import { Router } from '@angular/router'
import { IUser } from './user';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';   //HttpErrorResponse is not being used. Instead, we're using the 'Response' imported library
import { Response } from '@angular/http';   //Http, 
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { catchError } from 'rxjs/operators/catchError';
import 'rxjs/add/Observable/throw';
//import { MessageService } from '../messages/message.service'; //Implement this for ANY service that requires messaging (OR LOGGING)

@Injectable()
export class AuthService {
    private _registerUrl = "http://localhost:53465/account/registeruser";
    private _loginUrl = "http://localhost:53465/account/login";
    private _logoutUrl = "http://localhost:53465/account/logout";

    redirectUrl: string;
    loggedInUser: IUser;
    isSessionLoggedIn: boolean;

    constructor(private _http: HttpClient, private _router: Router) {   //, private messageService: MessageService)
        this.isSessionLoggedIn = false;
    }

    registerUser(registeringUser): Observable<IUser> {
        let registrationResponse = this._http.post(this._registerUrl, registeringUser)
            //.map((response: Response) => <IUser>response.json())  //HttpClient.get() applies res.json() automatically and returns Observable<HttpResponse<string>>.
            //You no longer need to call the '.map' function above yourself.
            .pipe(catchError(error => this.handleError(error)));  //UPDATED the older way to 'catch'... previously was "  .catch(error => this.handleError(error));
        this.loggedInUser = registeringUser;
        return registrationResponse;
    }

    loginUser(loginUser): Observable<IUser> {
        let loginResponse = this._http.post(this._loginUrl, loginUser)
            //.map((response: Response) => <IUser>response.json())  //HttpClient.get() applies res.json() automatically and returns Observable<HttpResponse<string>>.
            //You no longer need to call the '.map' function above yourself.
            .pipe(catchError(error => this.handleError(error)));  //UPDATED the older way to 'catch'... previously was "  .catch(error => this.handleError(error));
        this.loggedInUser = loginUser;
        //this.loggedInUser.email = loginResponse.;
        return loginResponse;
    }

    logoutUser(): Observable<IUser> {
        let loginResponse = this._http.post(this._logoutUrl, this.loggedInUser)
            //.map((response: Response) => <IUser>response.json())  //HttpClient.get() applies res.json() automatically and returns Observable<HttpResponse<string>>.
            //You no longer need to call the '.map' function above yourself.
            .pipe(catchError(error => this.handleError(error)));  //UPDATED the older way to 'catch'... previously was "  .catch(error => this.handleError(error));
        return loginResponse;
    }

    logoutUserLocal(isRedirect) {
        this.loggedInUser = undefined;
        localStorage.removeItem('token');
        this.isSessionLoggedIn = false;
        if (isRedirect) {
            this._router.navigate(['/login']);
        }
    }

    getToken() {
        return localStorage.getItem('token');
    }

    loggedIn() {
        return (!!localStorage.getItem('token') && this.isSessionLoggedIn);
    }

    handleError(error: Response) {
        //Change this to pass the exception to some logging service
        console.error(error);
        if (error && error.status == 401) {
            this.logoutUserLocal(true);
        }

        //can PROBABLY include more status codes to handle here

        return Observable.throw(error);
    }
}
