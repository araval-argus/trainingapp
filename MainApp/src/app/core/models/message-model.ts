export interface  MessageModel{
  id?: number;
  message: string;
  sender: string;
  reciever: string;
  createdAt?: Date;
  isSeen?: boolean;
  repliedToMsg?: string;
  messageType?: number;
}
