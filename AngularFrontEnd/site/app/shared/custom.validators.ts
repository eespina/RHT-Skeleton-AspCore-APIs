import { AbstractControl } from '@angular/forms';

export class CustomValidators {

    ////Custom Validator (should probably be inside a different class)
    ////Instead of using 'ValidationErrors', we'll just use an Object with a key/value pair and it will look as "{[key: string] : any}"
    //function emailDomainValidator(control: AbstractControl): { [key: string]: any } | null {
    //    const email: string = control.value;
    //    const domain = email.substring(email.lastIndexOf('@') + 1); //should give us the email domain
    //    if (email === '' || domain.toLowerCase() === 'email.com') {    //just a test using 'email.com' as the desired domain
    //        return null;
    //    } else {
    //        return { 'emailDomainValidator': true };
    //    }
    //}

    //similar version to the COMMENTED out 'emailDomainValidator' function (above) but with a Closure
    static emailDomainValidator(domainName: string) {
    //function emailDomainValidator(domainName: string) { - this was the old way when it was in the reactiveForm component
        return (control: AbstractControl): { [key: string]: any } | null => {
            const email: string = control.value;
            const domain = email.substring(email.lastIndexOf('@') + 1); //should give us the email domain
            if (email === '' || domain.toLowerCase() === domainName.toLocaleLowerCase()) {    //just a test using 'email.com' as the desired domain
                return null;
            } else {
                return { 'emailDomainValidator': true };
            }
        };
    }

}