export interface Post {
  id: number;
  accoutId: number;
  topicId: number;
  nacestorId: number | null;
  commentsCount: number;
  content: string;
  createdAt: Date;
  deletedAt: Date;
}
