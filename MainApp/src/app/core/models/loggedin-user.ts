export interface LoggedInUser
{
    firstName?: string,
    lastName?: string,
    email?:string,
    //Username is stored in sub
    sub?: string,
    UserName?:string,
    Designation? : string
    ImagePath?:string
}
