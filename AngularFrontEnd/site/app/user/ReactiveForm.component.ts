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

    loadFakeDataClick(): void {
        //This is just fake data that exists so I don't have to sample data from the database (or any persisted information)
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

        //now we're also logging to the console through the following method
        this.logKeyValuePairs(this.reactiveFormGroup);
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

    //An example of looping through each control in the group. useful for Rest of controls, enable/disable form controls validation set/clears, mark dirty/touch/etc..
    logKeyValuePairs(group: FormGroup): void {
        //retreive all the keys we have in the group, and jsut prints them out to the log (notice it does NOT log the nested group)
        console.log(Object.keys(group.controls));

        //use a loop with a forEach
        Object.keys(group.controls).forEach((key: string) => {//get all the keys and loop over each key
            //the abstractControl variable can be, either, a FormControl or a NESTED FormGroup, so we need to check which it is
            const abstractControl = group.get(key); //get the reference to its associated control by using that key
            if (abstractControl instanceof FormGroup) {
                this.logKeyValuePairs(abstractControl);//recursively call the same method for the NESTED form group
            } else {
                console.log('key = ' + key + ' value = ' + abstractControl.value);
            }
        });
    }

    DisableNestFormClick() {    //example of disableing the NESTED controls only
        const group = this.reactiveFormGroup;
        Object.keys(group.controls).forEach((key: string) => {//use a loop with a forEach to get all the keys and loop over each key
            //the abstractControl variable can be, either, a FormControl or a NESTED FormGroup, so we need to check which it is
            const abstractControl = group.get(key); //get the reference to its associated control by using that key
            if (abstractControl instanceof FormGroup) {
                abstractControl.disable();  //this will disable the NESTED controls
            } else {
                //if it's not a nested form, it will mark it as dirty (not useful, but an example of being able to use other in-house techniques)
                abstractControl.markAsDirty();
            }
        });

    }
}
