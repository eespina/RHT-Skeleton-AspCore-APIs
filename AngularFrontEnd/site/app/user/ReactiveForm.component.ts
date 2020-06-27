import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators, AbstractControl, FormArray } from '@angular/forms';   //FormGroup and FormControl inherit from AbstractControl
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
        },
        'dynamicNestedGroupName': {
            'required': 'Requred Field'
        },
        'dynamicExperienceInYears': {
            'required': 'Requred Field'
        },
        'dynamicProficiency': {
            'required': 'Requred Field'
        }
    };

    //now that we have 'validationMessages' ready, we need this object to store the validation messages of the form controls that have actually failed validation
    //This is what the UI will actually bind to
    formErrors = {
        //'firstName': '',
        //'lastName': '',
        //'email': '',
        //'confirmEmail': '',
        //'emailGroup': '',
        //'phone': '',
        //'proficiency': '',
        //'dynamicNestedGroupName': '',
        //'dynamicExperienceInYears': '',
        //'dynamicProficiency': ''
    };    // no longer needed since the logValidationErrors method would take care of this at runtime

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
            }),
            dynamicNestedGroup: this.fb.array([
                this.addDynamicFormGroup()
            ])

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

        //-------------------------- FormArray Example
        const formArray = new FormArray([//can create a Formarray like this, with the 'new' keyword
            new FormControl('Glenn', Validators.required),
            new FormGroup({
                country: new FormControl('', Validators.required)
            }),
            new FormArray([])
        ]);

        console.log(formArray.length);  //should print out the number '3' for each indece in the formArray

        //iterate over each index (FormControl, FormGroup or FormArray) in the array
        for (const control of formArray.controls) {
            if (control instanceof FormControl) {
                //do something to the control that is a FormControl
            }
            if (control instanceof FormGroup) {
                //do something to the control that is a FormGroup
            }
            if (control instanceof FormArray) {
                //do something to the control that is a FormArray
            }
        }

        //it's more common to use arrays with like types, however, you can use them with mixed types like the 'formArray' examples
        const formBuilderArray = this.fb.array([
            new FormControl('Glenn', Validators.required),//name
            new FormControl('IT', Validators.required),//department
            new FormControl('', Validators.required)//Gender
        ]);

        //FormArray Useful METHODS
        //push - Inserts the control at the end of the array    (i.e "formBuilderArray.push(new FormControl('Caleb', Validators.required));")
        //insert - Inserts the control at the specified index in the array
        //removeAt - Removes the control at the specified index in the array
        //setControl - Replace an existing control at the specified index
        //at - Return the control at the specified index in the array   (i.e "formBuilderArray.at(3).value;" .... would return 'Caleb')

        //using a FormGroup to create the same group of FormControls

        //it's more common to use arrays with like types, however, you can use them with mixed types like the 'formArray' examples
        const formGroup = this.fb.group([
            new FormControl('Glenn', Validators.required),//name
            new FormControl('IT', Validators.required),//department
            new FormControl('', Validators.required)//Gender
        ]);

        //DIFFERENCES between a formGroup and a formArray (as in the similarities between the two groups above) are that 
        //formarray data is serialized as an array whereas a formgroup is serialized as an Object. FormArrays are usefull for dynamically creating groups and controls

        //The FormArray items (above) are designed not to show, I would imaginge it's just passing one of the groups (above) into the 'this.reactiveFormGroup.setValue()' method (below)

        //-------------------------- FormArray Example END


        //This is just fake data that exists so I don't have to sample data from the database (or any persisted information)
        this.reactiveFormGroup.setValue({   //  "setValue" would be useful for setting data loaded from some other material
            firstName: 'FakeFirstname',
            lastName: '',
            userName: 'FakeUserName',
            phone: '1234568',
            password: 'FakePassword',
            contactPreference: '',

            //Nested Form Group Examples (not yet persisted in any kind of memory)
            nestedGroup: {
                nestedGroupName: 'FakenestedGroupName',
                experienceInYears: 1234,
                proficiency: ''
            },

            //Nested Form Group Examples (not yet persisted in any kind of memory)
            emailGroup: {
                email: 'email@email.com',
                confirmEmail: 'email@email.com'
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


            ////if the instance is a formArray, then we have to get inside the forgroup by recursively calling it with the FormGroup being passed in
            //if (abstractControl instanceof FormArray) {
            //    for (const control of abstractControl.controls) {
            //        //need to check if the control in the formarray is an instance of FormGroup, then recursively call the same method for the NESTED form group
            //        if (abstractControl instanceof FormGroup) {
            //            this.logValidationErrors(abstractControl);
            //        }
            //    }
            //} //no reason to need this anymore since the logic for dynamically generated 
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

    //dynamically add a dynamic formgrouping
    addDynamicFormGroup(): FormGroup {
        return this.fb.group({
            dynamicNestedGroupName: ['', Validators.required],
            dynamicExperienceInYears: ['', Validators.required],
            dynamicProficiency: ['', Validators.required]
        })
    }

    addDynamicGroupButton_Click(): void {
        (<FormArray>this.reactiveFormGroup.get('dynamicNestedGroup')).push(this.addDynamicFormGroup());   //need to type cast it into a FormArray to be able to use the 'push' method
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