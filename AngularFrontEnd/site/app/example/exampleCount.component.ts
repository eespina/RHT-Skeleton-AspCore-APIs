import { Component, Input, Output, EventEmitter } from '@angular/core';
@Component({
    selector: 'example-count',
    templateUrl: 'app/example/exampleCount.component.html',
    styleUrls: ['app/example/exampleCount.component.css']
})
export class ExampleCountComponent {

    selectedRadioButtonValue: string = 'All';

    @Output()   //A child component uses an event to pass data to the parent component, here we're passing a string to the Parent component (exampleList.component.ts)
    countRadioButtonSelectionChanged: EventEmitter<string> = new EventEmitter<string>();

    @Input()
    all: number;

    @Input()
    justOne: number;

    @Input()
    lessThanOne: number;

    onRadioButtonSelectionChanged() {
        this.countRadioButtonSelectionChanged.emit(this.selectedRadioButtonValue);
    }
}