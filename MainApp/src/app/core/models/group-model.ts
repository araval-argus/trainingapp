import { UserModel } from './UserModel';

export interface GroupModel{
  id: number;
  name: string;
  description?: string;
  groupIconUrl?: string;
  creatorUserName: string;
  loggedInUserIsAdmin?: boolean;
  lastMessage?: string;
  lastMessageTimeStamp?: Date;
}
