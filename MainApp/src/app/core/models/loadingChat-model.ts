export interface loadChatModel {
    id: number,
    sent: boolean
    content: string,
    sentAt: Date,
    replyToChat: number,
    replyToContent: string,
    isRead: number
}