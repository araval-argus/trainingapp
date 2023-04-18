export interface MessageModel
{
  content:string,
  messageFrom:string,
  messageTo:string,
  repliedTo:number,
  seen : number,
  type : string
}
