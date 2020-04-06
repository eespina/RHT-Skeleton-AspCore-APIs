import { Component, OnInit } from '@angular/core';
import { IExample } from './example';
import { ExampleService } from './example.service';
import { ExampleSingletonService } from './exampleSingleton.service';

@Component({
    selector: 'list-example',
    templateUrl: 'app/example/exampleList.component.html',
    styleUrls: ['app/example/exampleList.component.css']
})
export class ExampleListComponent {
    examples: IExample[];
    selectedExampleCountRadioButton: string = 'All';
    statusMessage: string = 'Loading Data. One Moment Please ...';

    constructor(private _exampleService: ExampleService, private _exampleSingletonService: ExampleSingletonService) { }

    get color(): string {
        return this._exampleSingletonService.colorPreference;
    }

    set color(value: string) {
        this._exampleSingletonService.colorPreference = value;
    }

    ngOnInit() {
        this._exampleService.getExamples()
            .subscribe((data) => this.examples = data,
            (error) => {
                this.statusMessage = 'Problem with the Service, Please Try Again Soon';
            });
    }

    getTotalExamplesCount(): number {
        return this.examples.length;
    }

    //Change these examples to align with the actual data.. we're comparing username but theres not a proper match with the actual data
    getTotalExamplesOneCount(): number {
        return this.examples.filter(e => e.userName === 'One').length;
    }

    //Change these examples to align with the actual data.. we're comparing username but theres not a proper match with the actual data
    getTotalExamplesLessThanOneCount(): number {
        return this.examples.filter(e => e.userName === 'Zero').length;
    }

    onExampleCountRadioButtonChange(selectedRadioButtonValue: string): void {
        this.selectedExampleCountRadioButton = selectedRadioButtonValue;
    }
}