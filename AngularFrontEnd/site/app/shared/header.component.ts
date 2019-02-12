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
            this._auth.logoutUser();
            document.getElementById('loginLogoutPlaceholder').innerText = "Log In";
        } else {
            if (this._router) {
                this._router.navigate(['/login']);
            }
        }
    }
}
