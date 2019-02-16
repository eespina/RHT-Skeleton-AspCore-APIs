import { Component, OnInit } from '@angular/core';
import { AuthService } from '../user/auth.service';
import { Router } from '@angular/router'
import { IUser } from '../user/user';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html'
})
export class LoginComponent implements OnInit {
    loginUserInfo: IUser = { username: '', password: '' } as IUser;
    constructor(private _auth: AuthService, private _router: Router) { }

    ngOnInit() { }

    loginUser() {
        if (this.loginUserInfo.username != '' && this.loginUserInfo.password != '') {
            this._auth.loginUser(this.loginUserInfo)
                .subscribe(
                res => {
                    localStorage.setItem('token', res.token)
                    document.getElementById('loginLogoutPlaceholder').innerText = "Log Out";
                    this._router.navigate(['/home'])
                },
                err => {
                    localStorage.removeItem('token')//probably wont have a token anyway, but whatever.
                    document.getElementById('serverError').style.visibility = 'visible';
                    document.getElementById('usernameError').style.visibility = 'hidden';
                    document.getElementById('passwordError').style.visibility = 'hidden';
                });
            this._auth.isSessionLoggedIn = true;
        }

        if (this.loginUserInfo.username == '') {
            document.getElementById('usernameError').style.visibility = 'visible';
            document.getElementById('serverError').style.visibility = 'hidden';
        } else {
            document.getElementById('usernameError').style.visibility = 'hidden';
        }
        if (this.loginUserInfo.password == '') {
            document.getElementById('passwordError').style.visibility = 'visible';
            document.getElementById('serverError').style.visibility = 'hidden';
        } else {
            document.getElementById('usernameError').style.visibility = 'hidden';
        }
    }
}