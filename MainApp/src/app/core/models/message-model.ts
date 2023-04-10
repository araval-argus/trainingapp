export interface  MessageModel{
  message: string;
  sender: string;
  reciever: string;
  createdAt?: Date;
  isSeen?: boolean;
}
