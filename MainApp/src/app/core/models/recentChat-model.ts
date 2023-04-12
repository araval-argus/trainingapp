import { Profile } from "./profile-model";

export interface recentChat {
    to: any,
    chatContent: {
        content: string,
        sentAt: Date,
        type: string
    }
    unreadCount: number
}