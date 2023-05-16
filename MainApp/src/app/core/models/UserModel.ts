import { DesignationModel } from './designation-model';

export interface UserModel {
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  imageUrl: string;
  designation: DesignationModel;
  lastMessage?: string;
  lastMessageTimeStamp?: Date;
  unreadMessageCount?: number;
  status: {
    id: number,
    type: string,
    tagClasses: string,
    tagStyle: string
  }
}
