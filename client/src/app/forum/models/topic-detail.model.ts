import { Post } from "src/shared/models/post-model";
import { Topic } from "./topic.model";
import { User } from "src/shared/models/user.model";
import { PostInfo } from "src/shared/models/post-info.model";

export interface TopicDetail {
  topic: Topic;
  posts: PostInfo;
  post: Post;
  sender: User;
}
