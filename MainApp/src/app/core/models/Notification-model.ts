export interface NotificationModel{
  id: number;
  type: any;
  raisedFor: string;
  raisedBy: string;
  raisedInGroup?: string;
  createdAt: Date;
  messageToBeDisplayed: string;
}
