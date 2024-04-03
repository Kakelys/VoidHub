import { Topic } from "src/app/forum/models/topic.model";
import { Post } from "./post-model";
import { User } from "./user.model";

export interface PostInfo {
  post: Post;
  sender: User;
  topic: Topic
}
