import { Component, OnInit } from '@angular/core';
import { AuthService } from '../user/auth.service';
import { Router } from '@angular/router'
import { IUser } from '../user/user';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html'
})
export class RegisterComponent implements OnInit {

    registeringUser: IUser = { username: '', password: '' } as IUser;
    constructor(private _auth: AuthService, private _router: Router) { }

    ngOnInit() { }

    registerUser() {
        this._auth.registerUser(this.registeringUser)
            .subscribe(
            res => {
                localStorage.setItem('token', res.token)
                this._router.navigate(['/home'])
            },
            err => console.log(err)
            )
    }


}
