export interface Post {
  id: number;
  accoutId: number;
  topicId: number;
  nacestorId: number | null;
  commentsCount: number;
  likesCount: number;
  content: string;
  createdAt: Date;
  deletedAt: Date;

  isLiked: boolean;
}
