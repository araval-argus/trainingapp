import { LoggedInUser } from "./loggedin-user";

export interface RecentChatUsers {
    user: LoggedInUser,
    unreadCount: number,
    lastMessage: string
}
