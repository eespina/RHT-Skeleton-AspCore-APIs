import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';   //FormGroup and FormControl inherit from AbstractControl

@Component({
    selector: 'reactive-form-example',
    templateUrl: './reactiveForm.component.html'
})
export class ReactiveFormComponent implements OnInit {
    reactiveFormGroup: FormGroup;
    constructor(private fb: FormBuilder) { }

    ngOnInit() {
        this.reactiveFormGroup = this.fb.group({
            //create key/valkue pair (key is the name of the child control, and the value is an array)
            //1st element in the array is the default value (in this case, an empty string). The 2nd and 3rd parameters signify sync/async validators
            firstName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(42)]],
            lastName: [''],
            userName: [''],
            email: [''],
            password: [''],
            nestedGroup: this.fb.group({
                nestedGroupName: [''],
                experienceInYears: [''],
                proficiency: ['']
            })

            //when it comees to validators, there's 'required', 'requiredTrue', 'email', 'pattern', 'min', 'max', 'minLength', 'maxLength'
        });

        //firstName - subscribe to the valueChanges observable of firstName formControl
        this.reactiveFormGroup.get('firstName').valueChanges.subscribe(value => {
            //specify an annonymous function that gets executed everytime the value of the formControl changes
            console.log('firstName value changed to "' + value + '"');
        });

        //lastName - subscribe to the valueChanges observable of lastName formControl
        this.reactiveFormGroup.get('lastName').valueChanges.subscribe((val: string) => {
            console.log('lastName value length is ' + val.length);
        });

        //subscribe to the ROOT form group
        this.reactiveFormGroup.valueChanges.subscribe((jsonValue: any) => {   //'any' is the default type
            //specify an annonymous function that gets executed everytime the value of the formControl changes
            console.log('form changed to (json) of ' + JSON.stringify(jsonValue));
        });

        //subscribe to the NESTED form group
        this.reactiveFormGroup.get('nestedGroup').valueChanges.subscribe((jsonValue: any) => {   //'any' is the default type
            //specify an annonymous function that gets executed everytime the value of the formControl changes
            console.log('NESTED form changed to (json) of ' + JSON.stringify(jsonValue));
        });
    }

    /*  below is the technique of using Reractive forms WITHOUT FormBuilder. It only uses FormGroup and FormControl and it has a parameterless constructor.
        The FormControl is no longer in use and can be un-imported

    constructor() { }

    ngOnInit() {
        this.reactiveFormGroup = new FormGroup({
            firstName: new FormControl(),
            lastName: new FormControl(),
            userName: new FormControl(),
            email: new FormControl(),
            password: new FormControl(),

            //Nested Form Group Examples (not yet persisted in any kind of memory)
            nestedGroup: new FormGroup({
                nestedGroupName: new FormControl(),
                experienceInYears: new FormControl(),
                proficiency: new FormControl()
            })
        });
    }*/

    //This is just fake data that exists so I don;t have to sample data from the database (or any persisted information)
    loadFakeDataClick(): void {
        this.reactiveFormGroup.setValue({   //  "setValue" would be useful for setting data loaded from some other material
            firstName: 'FakeFirstname',
            lastName: 'FakeLastName',
            userName: 'FakeUserName',
            email: 'fake@email.com',
            password: 'FakePassword',

            //Nested Form Group Examples (not yet persisted in any kind of memory)
            nestedGroup: {
                nestedGroupName: 'FakenestedGroupName',
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
