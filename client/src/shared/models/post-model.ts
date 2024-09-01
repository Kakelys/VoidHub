export type Post = {
    id: number
    accountId: number
    topicId: number
    ancestorId: number | null
    commentsCount: number
    likesCount: number
    content: string
    createdAt: Date
    deletedAt: Date
    isLiked: boolean
}
