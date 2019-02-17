import { Component, OnInit } from '@angular/core';
import { AuthService } from '../user/auth.service';
import { Router } from '@angular/router'

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html'
})
export class HeaderComponent implements OnInit {
    constructor(private _auth: AuthService, private _router: Router) { }

    ngOnInit() { }

    loginOrOutUser() {
        if (this._auth.loggedIn()) {
            this._auth.logoutUser()
                .subscribe(
                res => {
                    document.getElementById('loginLogoutPlaceholder').innerText = "Log In";
                    this._auth.logoutUserLocal(false);// no need to redirect because it will happen after this method is true
                },
                err => {
                    //Log something HERE to somewhere
                    console.log('Error Logging Out User');
                    this._auth.handleError(err);
                });
        }
        if (this._router) {
            this._router.navigate(['/login']);
        }
    }
}
