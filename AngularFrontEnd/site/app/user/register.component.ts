import { Component, OnInit, ViewChild } from '@angular/core';
import { AuthService } from '../user/auth.service';
import { Router } from '@angular/router'
import { IUser } from '../user/user';
import { NgForm } from '@angular/forms';

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html'
})
export class RegisterComponent implements OnInit {

    registeringUser: IUser = {  //for whatever reason, this not being here (initialized) would error out and complain at runtime
        firstName: '', lastName: '', userName: '', password: '', administeringUserEmail: '',
        email: '', isActive: true, userType: { id: 0, name: '' }, id: 0, isAdmin: false, tokenHandleViewModel: { expiration: '', token: '' }
    } as IUser;

    @ViewChild('registrationForm') public createExampleForm: NgForm

    constructor(private _auth: AuthService, private _router: Router) { }

    ngOnInit() { }

    registerUser(exForm: NgForm) {
        this.registeringUser.userType = { id: 2, name:''};   //TODO - delete the 'name' property (OR, think of something else it would be useful for)
        this.registeringUser.administeringUserEmail = this._auth.loggedInUser.email;
        this.registeringUser.tokenHandleViewModel = this._auth.loggedInUser.tokenHandleViewModel;

        //keeps track of the created user after the exForm is RESET (this may not be completely necessary unless I have a redirect to the exampleList (user list) page)
        const exampleHolderObject: IUser = Object.assign({}, this.registeringUser);

        this._auth.registerUser(exampleHolderObject)
            .subscribe(
            res => {

                //Place anything else here useful
                document.getElementById('lblResult').innerHTML = 'User ' + res.firstName + '' + res.lastName + ' (' + res.userName + ') has been created!';
                document.getElementById('lblResult').style.visibility = 'visible';

                exForm.reset(); //reset the form after the for is used. resets data, ie dirty, valid, etc..
                //exForm.reset({ username: 'exampleName', isActive: false  }); //another version with default data being set after the reest
                //this.createExampleForm.reset();   //can also reset the form using this if you have the property. can be used if you do njot want to use a parameter in this method
            },
            err => {
                console.log(err)
                document.getElementById('lblResult').innerHTML = 'ERROR ' + err + '.';
                document.getElementById('lblResult').style.visibility = 'visible';
            });
    }
}
//TODO - get rid of all these "getElementById" calls and replace them with Angular-ization