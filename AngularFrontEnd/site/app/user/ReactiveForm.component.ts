import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
    selector: 'reactive-form-example',
    templateUrl: './reactiveForm.component.html'
})
export class ReactiveFormComponent implements OnInit {
    reactiveFormGroup: FormGroup;
    constructor() { }

    ngOnInit() {
        this.reactiveFormGroup = new FormGroup({
            firstName: new FormControl(),
            lastName: new FormControl(),
            userName: new FormControl(),
            email: new FormControl(),
            password: new FormControl(),

            //Nested Form Group Examples (not yet persisted in any kind of memory)
            skills: new FormGroup({
                skillName: new FormControl(),
                experienceInYears: new FormControl(),
                proficiency: new FormControl()
            })
        });
    }

    onSubmit(): void {
        console.log(this.reactiveFormGroup.value);  //right now, just prints the object
    }
}
