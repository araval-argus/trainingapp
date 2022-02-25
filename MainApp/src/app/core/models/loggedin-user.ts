export interface LoggedInUser
{
    id?: number,
    firstName?: string,
    lastName?: string,
    email?:string,
    userName?: string,
    profileType?: number,
    createdAt?: Date,
    createdBy?: number,
    lastUpdatedAt?: Date,
    lastUpdatedBy?: number,
    sub?: string,
    profileUrl?: string
}
