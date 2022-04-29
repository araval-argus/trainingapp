import { LoggedInUser } from "./loggedin-user";
import { Chat } from "./chat";

export interface RecentChatUsers {
    user: LoggedInUser,
    unreadCount: number,
    lastMessage: string,
    lastMessageType: string
}
