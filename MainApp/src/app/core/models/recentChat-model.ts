import { Profile } from "./profile-model";

export interface recentChat {
    to: any,
    chatContent: {
        content: string,
        sentAt: Date
    }
    unreadCount: number
}