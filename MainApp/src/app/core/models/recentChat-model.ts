import { Profile } from "./profile-model";

export interface recentChat {
    to: string,
    chatContent: {
        content: string,
        sentAt: Date
    }
}