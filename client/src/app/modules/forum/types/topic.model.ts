export type Topic = {
    id: number
    forumId: number
    title: string
    createdAt: Date
    deletedAt: Date | null
    isClosed: boolean
    isPinned: boolean
    postsCount: number
    commentsCount: number
}
