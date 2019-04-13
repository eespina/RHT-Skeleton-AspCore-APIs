import { AfterViewInit, Component, OnInit } from '@angular/core';
import { AuthService } from '../user/auth.service';
import { Router } from '@angular/router'

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html'
})
export class HeaderComponent implements OnInit {
    mobileW: string = '767';
    tabletW: string = '992';
    isPhone: boolean;
    isTablet: boolean;

    constructor(private _auth: AuthService, private _router: Router) { }

    ngOnInit() {
        /* jshint latedef: nofunc */
        //window.history.pushState("", "", '/');
    }

    ngAfterViewInit() {
        this.isPhone = window.matchMedia("(min-width: " + this.mobileW + "px)").matches;
        this.isTablet = window.matchMedia("(min-width: " + this.tabletW + "px)").matches;

        if (this.isMobile()) {
            this.mobileNavigationHandler();
            this.mobileScrollToTopClick();
        }
    }

    isMobile(): boolean {
        return !this.isPhone && !this.isTablet;
    }

    mobileNavigationHandler() {
        var btnMobileHamburger = document.getElementById('btn-nav-menu');//mobile-button-section
        var divPrimaryNav = document.getElementById('primary-nav');
        // var divSecondaryNav = document.getElementsByClassName('secondary-nav')[0];

        divPrimaryNav.className += 'hide';

        btnMobileHamburger.addEventListener('click', function (e) {
            divPrimaryNav.classList.toggle('hide');
            btnMobileHamburger.classList.toggle('open');
            divPrimaryNav.classList.toggle('open');
            // divSecondaryNav.classList.toggle('open');
        }, false);

        if (document.getElementsByClassName("nav-right-list")) {
            //hamburger menu list item click event to send to top of the page
            for (var i = 0; i < document.getElementsByClassName('nav-right-list')[0].getElementsByTagName('li').length; i++) {
                if (document.getElementsByClassName('nav-right-list')[0].getElementsByTagName('li')[i].getElementsByTagName('a').length > 0) {
                    document.getElementsByClassName('nav-right-list')[0].getElementsByTagName('li')[i].getElementsByTagName('a')[0]
                        .addEventListener('click', function (e) {
                            divPrimaryNav.classList.toggle('hide');
                            document.getElementById('btn-nav-menu').classList.remove('open');
                            document.getElementById('primary-nav').classList.remove('open');

                            ////CURRENTLY dictates animation for the page (can be used as an example)
                            //document.querySelector('.home-hero').scrollIntoView({
                            //    behavior: 'smooth'
                            //});

                            ////CURRENTLY dictates animation for the page (can be used as an example)
                            //document.querySelector('.home-about').scrollIntoView({
                            //    behavior: 'smooth'
                            //});
                        }, false);
                }
            }

        }
    }

    mobileScrollToTopClick() {
        var mobileScrollToTopArea = document.getElementById('mobile-button-section');
        if (mobileScrollToTopArea) {
            mobileScrollToTopArea.addEventListener('click', function () {
                window.scroll({
                    top: 0,
                    left: 0,
                    behavior: 'smooth'
                });
            });
        }
    }

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
