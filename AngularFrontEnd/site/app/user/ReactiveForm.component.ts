import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';    //can also use "FormBuilder", alongside constructor injection, to create a form similar looking to the current manner

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

    //This is just fake data that exists so I don;t have to sample data from the database (or any persisted information)
    loadFakeDataClick(): void {
        this.reactiveFormGroup.setValue({   //  "setValue" would be useful for setting data loaded from some other material
            firstName: 'FakeFirstname',
            lastName: 'FakeLastName',
            userName: 'FakeUserName',
            email: 'fake@email.com',
            password: 'FakePassword',

            //Nested Form Group Examples (not yet persisted in any kind of memory)
            skills: {
                skillName: 'FakeSkill',
                experienceInYears: 1234,
                proficiency: 'advanced'
            }
        });
    }

    //This is the PATCH version that would NOT include the nested values. If you used the setValue in the "loadFakeDataClick()" function,
        //without the nested values, it will complain about not having the nested elements included
    loadFakeDataPatchClick(): void {
        this.reactiveFormGroup.patchValue({
            firstName: 'FakeFirstname',
            lastName: 'FakeLastName',
            userName: 'FakeUserName',
            email: 'fake@email.com',
            password: 'FakePassword'
        });
    }

    onSubmit(): void {
        console.log(this.reactiveFormGroup.value);  //right now, just prints the object
    }
}
