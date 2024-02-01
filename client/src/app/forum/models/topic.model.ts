export interface Topic {
  id: number;
  forumId: number;
  title: string;
  createdAt: Date;
  isClosed: boolean;
  isPinned: boolean;
  postsCount: number;
  commentsCount: number;
}
