import { Component } from '@angular/core';
import { ExampleSingletonService } from '../example/exampleSingleton.service';

@Component({
    templateUrl: './home.component.html'
    //template: `
    //    <h1>This is the HOME Page</h1>
    //    <div>
    //        Color Preference : <input type='text' [(ngModel)]='color' [style.background]='color' />
    //    </div>
    //`
})
export class HomeComponent {

    constructor(private _exampleSingletonService: ExampleSingletonService) { }

    ngOnInit() {
        this.homeComponentHandler();
        //window.history.pushState("", "", '/');
    }

    homeComponentHandler() {

        ////NOT sure what this is about exactly and why it is still here
        //window.onscroll = function () {
        //    var homeAboutSectionLocation = document.getElementsByClassName('home-about')[0].getBoundingClientRect.length;
        //    if (homeAboutSectionLocation < 600) {
        //        //document.getElementsByClassName('home-about-bg-img')[0].style.display = 'block';
        //        //document.getElementsByClassName('home-content')[0].style.display = 'block';
        //        document.getElementsByClassName('home-about-bg-img')[0];
        //        document.getElementsByClassName('home-content')[0];
        //    }
        //};
    }

    get color(): string {
        return this._exampleSingletonService.colorPreference;
    }

    set color(value: string) {
        this._exampleSingletonService.colorPreference = value;
    }
}