export interface MessageDisplayModel{
  id : number,
  content : string,
  messageFrom : string,
  messageTo : string,
  createdAt : Date,
  repliedTo : string,
  seen : number,
  type : string
}
