export interface LoggedInUser
{
    firstName?: string,
    lastName?: string,
    email?:string,
    //Username is stored in sub
    sub?: string,
    userName?:string,
    designation? : string
    imagePath?:string
}
