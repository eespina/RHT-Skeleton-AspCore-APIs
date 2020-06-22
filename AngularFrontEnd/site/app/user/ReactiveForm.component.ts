import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators, AbstractControl } from '@angular/forms';   //FormGroup and FormControl inherit from AbstractControl
import { CustomValidators } from '../shared/custom.validators';

@Component({
    selector: 'reactive-form-example',
    templateUrl: './reactiveForm.component.html'
})
export class ReactiveFormComponent implements OnInit {
    reactiveFormGroup: FormGroup;

    //example showing how to use the Component class to hold the validation syntax instead of having it inside the .html
    validationMessages = {
        'firstName': {
            'required': 'Required',
            'minLength': 'Too Short',  //shows 'undefined' at the moment
            'maxLength': 'Too Long' //shows 'undefined' at the moment
        },
        'lastName': {
            'required': 'Required',
            'minLength': 'Too Short',   //shows 'undefined' at the moment
            'maxLength': 'Too Long' //shows 'undefined' at the moment
        },
        'email': {
            'required': 'Email is Required',
            'email': 'Email must be valid',
            'emailDomainValidator': 'Email must be a "email" domain'
        },
        'confirmEmail': {
            'required': 'Confirm Email is Required',
            'email': 'Email must be valid'
        },
        'emailGroup': {
            'emailMisMatch': 'Emails do NOT match'
        },
        'phone': {
            'required': 'Phone is Required'
        },
        'proficiency': {
            'required': 'Proficiency is Required'
        }
    };

    //now that we have 'validationMessages' ready, we need this object to store the validation messages of the form controls that have actually failed validation
    //This is what the UI will actually bind to
    formErrors = {
        'firstName': '',
        'lastName': '',
        'email': '',
        'confirmEmail': '',
        'emailGroup': '',
        'phone': '',
        'proficiency': ''
    };

    constructor(private fb: FormBuilder) { }

    ngOnInit() {
        this.reactiveFormGroup = this.fb.group({
            //create key/value pair (key is the name of the child control, and the value is an array)
            //1st element in the array is the default value (in this case, an empty string). The 2nd and 3rd parameters signify sync/async validators
            firstName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(42)]],
            lastName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(42)]],
            userName: [''],
            contactPreference: ['email'],
            emailGroup: this.fb.group({
                email: ['', [Validators.required, Validators.email, CustomValidators.emailDomainValidator('email.com')]],
                confirmEmail: ['', Validators.required]
            }, { validator: matchEmailValidator }),//tie the customer validator function to the nested form group
            phone: [''],
            password: [''],
            nestedGroup: this.fb.group({
                nestedGroupName: [''],
                experienceInYears: ['6'],   /// '6' is an example of using the default value
                proficiency: ['', Validators.required]
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

        //automatically do the validation logic through subscription
        this.reactiveFormGroup.valueChanges.subscribe((data) => {
            this.logValidationErrors(this.reactiveFormGroup);
        });

        //Every time the contact preference changes, this method, below, would get called. This is better for unit testing as the binding/calling method is NOT in the HTML anymore
        this.reactiveFormGroup.get('contactPreference').valueChanges.subscribe((data: string) => {
            this.onContactPreference_Changed(data);
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
            phone: new FormControl(),
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
            lastName: '',
            userName: 'FakeUserName',
            email: 'fake@email.com',
            phone: '1234568',
            password: 'FakePassword',

            //Nested Form Group Examples (not yet persisted in any kind of memory)
            nestedGroup: {
                nestedGroupName: 'FakenestedGroupName',
                experienceInYears: 1234,
                proficiency: ''
            }
        });

        //now we're also logging to the console through the following method
        this.logValidationErrors(this.reactiveFormGroup);
        console.log(this.formErrors);
    }

    //This is the PATCH version that would NOT include the nested values. If you used the setValue in the "loadFakeDataClick()" function,
    //without the nested values, it will complain about not having the nested elements included
    loadFakeDataPatchClick(): void {
        this.reactiveFormGroup.patchValue({
            firstName: 'FakeFirstname',
            lastName: 'FakeLastName',
            userName: 'FakeUserName',
            email: 'fake@email.com',
            phone: '1234568',
            password: 'FakePassword'
        });
    }

    onSubmit(): void {
        console.log(this.reactiveFormGroup.value);  //right now, just prints the object
    }

    //An example of looping through each control in the group. useful for Rest of controls, enable/disable form controls validation set/clears, mark dirty/touch/etc..
    logValidationErrors(group: FormGroup = this.reactiveFormGroup): void {   //use " = this.reactiveFormGroup" to set it as the default value. doing this makes us not have to specify a value for this parameter when we call it from the template 
        //retreive all the keys we have in the group, and just prints them out to the log (notice it does NOT log the nested group)
        console.log(Object.keys(group.controls));

        //use a loop with a forEach
        Object.keys(group.controls).forEach((key: string) => {//get all the keys and loop over each key

            //the abstractControl variable can be, either, a FormControl or a NESTED FormGroup, so we need to check which it is
            const abstractControl = group.get(key); //get the reference to its associated control by using that key

            this.formErrors[key] = '';
            if (abstractControl) {
                if (!abstractControl.valid && (abstractControl.touched || abstractControl.dirty)) {
                    const messages = this.validationMessages[key];

                    for (const errorKey in abstractControl.errors) {
                        if (errorKey) {
                            this.formErrors[key] += messages[errorKey] + ' ';
                        }
                    }
                }
            }

            if (abstractControl instanceof FormGroup) {
                this.logValidationErrors(abstractControl);   //recursively call the same method for the NESTED form group
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

    //dynamically set and clear validators on specific controls
    onContactPreference_Changed(selectedValue: string) {
        const phoneControl = this.reactiveFormGroup.get('phone');
        if (selectedValue === 'phone') {
            if (Validators) {
                phoneControl.setValidators([Validators.required, Validators.minLength(5)]);
            }
        } else {
            phoneControl.clearValidators();
        }

        phoneControl.updateValueAndValidity();  //immediately triggers the validation. in this example, we want this for forcing the user to enter a phone
    }
}

//Returns an object with a Key(string)/Value(any) pair if there is a validation error. If there's no error, it will return null 
function matchEmailValidator(group: AbstractControl): { [key: string]: any } | null {
    //we'll opass our nested formgroup (emailFormGroup)
    const emailControl = group.get('email');
    const confirmEmailControl = group.get('confirmEmail');

    //prestine means the user didn't have an opportunity to start typeing in the confrim email form conrtol
    if (emailControl.value === confirmEmailControl.value || confirmEmailControl.pristine) {
        return null;
    } else {
        return { 'emailMisMatch': true };
    }
}