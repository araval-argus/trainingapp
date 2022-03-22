export interface Chat {
    id?: number
    messageFrom?: string
    messageTo?: string
    type?: string
    replyTo?: string
    content?: string
    createdAt?: Date
    updatedAt?: Date
    deletedAt?: Date
}
