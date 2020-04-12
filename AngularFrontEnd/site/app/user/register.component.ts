import { Component, OnInit, ViewChild } from '@angular/core';
import { AuthService } from '../user/auth.service';
import { Router } from '@angular/router'
import { IUser } from '../user/user';
import { NgForm } from '@angular/forms';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html'
})
export class RegisterComponent implements OnInit {

    registeringUser: IUser = {  //for whatever reason, this not being here (initialized) would error out and complain at runtime
        firstName: '', lastName: '', userName: '', password: '', administeringUserEmail: '',
        email: '', isActive: true, userType: { id: 0, name: '' }, id: 0, isAdmin: false, tokenHandleViewModel: { expiration: '', token: '' }
    } as IUser;

    @ViewChild('registrationForm') public createExampleForm: NgForm

    constructor(private _auth: AuthService, private _router: Router) { }

    ngOnInit() { }

    registerUser() {
        this.registeringUser.userType = { id: 2, name:''};   //TODO - delete the 'name' property (OR, think of something else it would be useful for)
        this.registeringUser.administeringUserEmail = this._auth.loggedInUser.email;
        this.registeringUser.tokenHandleViewModel = this._auth.loggedInUser.tokenHandleViewModel;
        this._auth.registerUser(this.registeringUser)
            .subscribe(
            res => {
                //Place anything here useful
                document.getElementById('lblResult').innerHTML = 'User ' + res.firstName + '' + res.lastName + ' (' + res.userName + ') has been created!';
                document.getElementById('lblResult').style.visibility = 'visible';
            },
            err => {
                console.log(err)
                document.getElementById('lblResult').innerHTML = 'ERROR ' + err + '.';
                document.getElementById('lblResult').style.visibility = 'visible';
            });
    }
}
