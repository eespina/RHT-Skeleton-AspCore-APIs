function mobileNavigationHandler() {
    var btnMobileHamburger = document.getElementById('btn-nav-menu');
    var divPrimaryNav = document.getElementsByClassName('primary-nav')[0];
    var divSecondaryNav = document.getElementsByClassName('secondary-nav')[0];

    if (!isDesktop()) {
        btnMobileHamburger.classList.remove('open');
        divPrimaryNav.classList.remove('open');
        divSecondaryNav.classList.remove('open');
        btnMobileHamburger.addEventListener('click', function (e) {
            btnMobileHamburger.classList.toggle('open');
            divPrimaryNav.classList.toggle('open');
            divSecondaryNav.classList.toggle('open');
        }, false);
    } else {
        if (!btnMobileHamburger.classList.contains('open')) {
            btnMobileHamburger.className += ' open';
            divPrimaryNav.className += ' open';
            divSecondaryNav.className += ' open';
        }
    }
}

function readyHeader() {
    mobileNavigationHandler();
    addEvent(window, "resize", function (event) {
        mobileNavigationHandler();
    });
}