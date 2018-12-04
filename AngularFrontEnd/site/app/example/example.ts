export interface IExample {

    emailString: string;
    firstNameString: string;
    lastNameString: string;
    UserNameString: string;
    createDate: Date;
    isActiveBoolean: Boolean;

    exampleId: number;
    //exampleString: string;
    //exampleNumber: number;
    //optionalExampleProperty?: string;

    ////getExampleNumber(exampleNumber: number): number;
}

//export class Example implements IExample {
//    getExampleNumber(exampleNumber: number): number {
//        return this.exampleNumber;
//    }
//    constructor(public exampleId: number, public exampleString: string, public exampleNumber: number) { }
//}