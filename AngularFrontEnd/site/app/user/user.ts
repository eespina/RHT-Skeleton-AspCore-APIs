export interface IUser {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    userName: string;
    password: string;
    userType: IUserType;
    administeringUserEmail: string
    tokenHandleViewModel: ITokenInfo;

    isAdmin: boolean;
    isActive: boolean;

}

export interface IUserType {
    id: number;
    name: string;
}

export interface ITokenInfo {
    token: string;
    expiration: string;
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