import { Component } from '@angular/core';
import { ExampleSingletonService } from '../example/exampleSingleton.service';

@Component({
    template: `
        <h1>This is the HOME Page</h1>
        <div>
            Color Preference : <input type='text' [(ngModel)]='color' [style.background]='color' />
        </div>
    `
})
export class HomeComponent {

    constructor(private _exampleSingletonService: ExampleSingletonService) { }

    get color(): string {
        return this._exampleSingletonService.colorPreference;
    }

    set color(value: string) {
        this._exampleSingletonService.colorPreference = value;
    }
}