import { Injectable } from '@angular/core';
import { Router } from '@angular/router'
import { IUser } from './user';
import { HttpClient } from '@angular/common/http';
import { Response } from '@angular/http';   //Http, 
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

    constructor(private _http: HttpClient, private _router: Router) //, private messageService: MessageService)
    {
        localStorage.removeItem('token')
    }

    registerUser(registeringUser): Observable<IUser> {
        let registrationResponse = this._http.post(this._registerUrl, registeringUser)
            //.map((response: Response) => <IUser>response.json())  //HttpClient.get() applies res.json() automatically and returns Observable<HttpResponse<string>>.
            //You no longer need to call the '.map' function above yourself.
            .catch(this.handleError);
        return registrationResponse
    }

    loginUser(loginUser): Observable<IUser> {
        let loginResponse = this._http.post(this._loginUrl, loginUser)
            //.map((response: Response) => <IUser>response.json())  //HttpClient.get() applies res.json() automatically and returns Observable<HttpResponse<string>>.
            //You no longer need to call the '.map' function above yourself.
            .catch(this.handleError);
        return loginResponse;
    }

    logoutUser() {
        localStorage.removeItem('token')
        this._router.navigate(['/login'])
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
