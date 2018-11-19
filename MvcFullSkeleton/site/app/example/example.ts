export interface IExample {
    exampleId: number;
    exampleString: string;
    exampleNumber: number;
    optionalExampleProperty?: string;

    //getExampleNumber(exampleNumber: number): number;
}

//export class Example implements IExample {
//    getExampleNumber(exampleNumber: number): number {
//        return this.exampleNumber;
//    }
//    constructor(public exampleId: number, public exampleString: string, public exampleNumber: number) { }
//}