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
        this._auth.loginUser(this.loginUserInfo)
            .subscribe(
            res => {
                localStorage.setItem('token', res.token)
                this._router.navigate(['/home'])
            },
            err => console.log(err)
            )
    }

}
