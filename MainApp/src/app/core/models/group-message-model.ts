export interface GroupMessageModel{
  id?: number;
  message: string;
  senderUserName: string;
  senderImage?: string;
  groupId: number;
  messageType?: number;
  createdAt?: Date;
  repliedToMsg?: string;
}
