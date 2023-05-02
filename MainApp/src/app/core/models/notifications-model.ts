export interface NotificationsModel
{
  id:number,
  msgFrom:string,
  msgTo:string,
  groupId?:number,
  content:string,
  createdAt:Date,
  fromImage:string
}
