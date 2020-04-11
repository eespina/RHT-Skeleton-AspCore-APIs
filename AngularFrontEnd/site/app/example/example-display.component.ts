import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { IExample } from './example';

@Component({
    selector: 'list-display-example',
    templateUrl: 'app/example/example-display.component.html',
    styleUrls: ['app/example/example-display.component.css']
})
export class ExampleDisplayComponent implements OnInit, OnChanges {
    @Input() exampleId: number;
    @Input() example: IExample;   //use the @Input attribute to let the parent class/componenet pass information into 
    constructor() { }
    ngOnInit() { }

    //NOT implemented, because we're not switching examples. The example I'm following uses an individual example instead of a list. The below still gets logged on component_load
    ngOnChanges(changes: SimpleChanges) {   //automatcially called whenever any of the @Input properties change
        //Demonstrates 'ngOnChanges' way of reacting to a detection change from the Parent to the Child component
        for (var p of Object.keys(changes)) {
            var change = changes[p];
            var from = JSON.stringify(change.previousValue);
            var to = JSON.stringify(change.currentValue);
            console.log('changes from ' + from + ' to ' + to + '.');    //TODO - Get rid of this console logging.
        }
    }
}
