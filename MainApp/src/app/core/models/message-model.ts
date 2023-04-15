export interface  MessageModel{
  id?: number;
  message: string;
  senderUserName: string;
  recieverUserName: string;
  createdAt?: Date;
  isSeen?: boolean;
  repliedToMsg?: string;
  messageType?: number;
}
