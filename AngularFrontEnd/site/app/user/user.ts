export interface IUser {
    id: number;
    username: string;
    isAdmin: boolean;
    token: string;
    password: string;

    email: string;
    firstName: string;
    lastName: string;
    createDate: Date;
    isActive: Boolean;
}

//export class User implements IUser {
//    id: number;
//    username: string;
//    email: string;
//    isAdmin: boolean;
//    token: string;
//    password: string;

//    firstName: string;
//    lastName: string;
//    createDate: Date;
//    isActive: Boolean;

//    //    getExampleUser(exampleUser: User): string {
//    //        return this.exampleUser;
//    //    }
//    //constructor(public id: number, public username: string, public email: string) { }
//}