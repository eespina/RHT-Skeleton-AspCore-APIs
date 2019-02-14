import { Component, OnInit } from '@angular/core';
import { AuthService } from '../user/auth.service';
import { Router } from '@angular/router'
import { IUser } from '../user/user';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html'
})
export class RegisterComponent implements OnInit {

    registeringUser: IUser = {  //for whatever reason, this not being here (initialized) would error out and complain at runtime
        firstName: '', lastName: '', username: '', password: '', email: '', isActive: true, userType: { id: 0, name: '' }, id: 0, isAdmin: false, token: ''
    } as IUser;

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
