import { Component, OnInit, Input } from '@angular/core';
import { IExample } from './example';

@Component({
    selector: 'list-display-example',
    templateUrl: 'app/example/exampleList-display.component.html',
    styleUrls: ['app/example/exampleList-display.component.css']
})
export class ExampleListDisplayComponent implements OnInit {
    @Input() example: IExample;   //use the @Input attribute to let the parent class/componenet pass information into 
    constructor() { console.log('inside the ExampleListDisplayComponent constructor method'); }
    ngOnInit() { console.log('inside the ExampleListDisplayComponent ngOnInit method'); }
}
