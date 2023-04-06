export interface LoggedInUser
{
    firstName?: string,
    lastName?: string,
    email?:string,
    userName?: string,
    // userName is stored as sub in token
    sub?: string,
    imageUrl?:string
}
