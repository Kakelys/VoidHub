import { Post } from "./post.model";
import { Topic } from "./topic.model";

export interface TopicDetail extends Topic {
  post: Post;
}
