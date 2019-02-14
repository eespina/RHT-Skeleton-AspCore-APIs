export interface IUser {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    username: string;
    password: string;
    userType: IUserType;

    isAdmin: boolean;
    token: string;
    isActive: boolean;
}

export interface IUserType {
    id: number;
    name: string;
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