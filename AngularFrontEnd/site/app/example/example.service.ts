﻿import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { IExample } from './example';
import { Http, Response } from '@angular/http';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { AuthService } from '../user/auth.service';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
//import 'rxjs/add/Observable/of';    //perhaps ANOTHER way of doing a get from somewhere
import 'rxjs/add/Observable/throw';
//import 'rxjs/add/operator/toPromise';

@Injectable()
export class ExampleService {
    constructor(private _http: HttpClient, private _router: Router, private _auth: AuthService) { }

    getExamples(): Observable<IExample[]> {
        var examples = this._http.get('http://localhost:53465/api/examples').delay(4130)    //delay is just used to test the loading words and css animation
            //.map((response: Response) => <IExample[]>response.json())
            //HttpClient.get() applies res.json() automatically and returns Observable<HttpResponse<string>>. You no longer need to call the '.map' function above yourself.
            .catch(error => this._auth.handleError(error));

        //perhaps ANOTHER way of facilitating a get from somewhere (implements "import 'rxjs/add/Observable/of'; " from above)
            //return Observable.of();

        return examples;
    }

    getExampleById(exId: string): Observable<IExample> {
        return this._http.get('http://localhost:53465/api/examples/' + exId)
            //.map((response: Response) => <IExample>response.json())
            //HttpClient.get() applies res.json() automatically and returns Observable<HttpResponse<string>>. You no longer need to call the '.map' function above yourself.
            .catch(error => this._auth.handleError(error));
    }

    //handleError(error: Response) {
    //    //Change this to pass the exception to some logging service
    //    console.error(error.status + ' - ' + error.statusText);
    //    if (error && error.status == 401) {
    //        if (this._auth) {
    //            this._auth.logoutUser();
    //        }
    //    }
    //    return Observable.throw(error);
    //}

    ////Promise technique
    //getExampleById(exId: string): Promise<IExample> {
    //    return this._http.get('http://localhost:42917/api/examples/' + exId)
    //        .map((response: Response) => <IExample>response.json())
    //        .toPromise()
    //        .catch(this.handlePromiseError);
    //}

    ////Promise technique
    //handlePromiseError(error: Response) {
    //    //Change this to pass the exception to some logging service
    //    console.error(error);
    //    throw (error);
    //}
}