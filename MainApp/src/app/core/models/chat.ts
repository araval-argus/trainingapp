export interface Chat {
    id?: number
    messageFrom?: number
    messageTo?: number
    type?: string
    content?: string
    createdAt?: Date
    updatedAt?: Date
    deletedAt?: Date
}
