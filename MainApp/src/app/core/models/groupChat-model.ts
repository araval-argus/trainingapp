export interface GroupChatModel {
    Id?: number,
    Content: string,
    GroupId: number,
    Sender: string,
    time: Date,
    ReplyingContent: string,
    ReplyingId: number,
    Type: string,
}