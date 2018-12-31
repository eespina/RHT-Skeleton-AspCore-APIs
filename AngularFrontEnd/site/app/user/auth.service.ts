import { Injectable } from '@angular/core';
import { Router } from '@angular/router'
import { IUser } from './user';

import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/Observable/throw';
//import { MessageService } from '../messages/message.service'; //Implement this for ANY service that requires messaging (OR LOGGING)

@Injectable()
export class AuthService {
    private _registerUrl = "http://localhost:53465/api/examples";
    private _loginUrl = "http://localhost:53465/account/login";

    registeringUser: IUser;
    redirectUrl: string;

    constructor(private _http: Http, private _router: Router) { }   //, private messageService: MessageService) { }

    registerUser(registeringUser): Observable<IUser> {
        let registrationResponse = this._http.post(this._registerUrl, registeringUser)
            .map((response: Response) => <IUser>response.json())
            .catch(this.handleError);
        return registrationResponse
    }

    loginUser(loginUser): Observable<IUser> {
        let loginResponse = this._http.post(this._loginUrl, loginUser)
            .map((response: Response) => <IUser>response.json())
            .catch(this.handleError);
        return loginResponse;
    }

    logoutUser() {
        localStorage.removeItem('token')
        this._router.navigate(['/events'])
    }

    getToken() {
        return localStorage.getItem('token')
    }

    loggedIn() {
        return !!localStorage.getItem('token')
    }

    handleError(error: Response) {
        //Change this to pass the exception to some logging service
        console.error(error);
        return Observable.throw(error);
    }
}
