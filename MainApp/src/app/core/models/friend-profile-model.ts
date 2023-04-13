import { DesignationModel } from './designation-model';

export interface FriendProfileModel {
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  imageUrl: string;
  designation: DesignationModel;
  lastMessage?: string;
  lastMessageTimeStamp?: Date;
  unreadMessageCount?: number;
}
