export interface LoggedInUserModel
{
    firstName?: string,
    lastName?: string,
    email?:string,
    userName?: string,
    // userName is stored as sub in token
    sub?: string,
    imageUrl?:string,
    designation?: string,
    status: {
      id: number,
      type: string
    }
}
