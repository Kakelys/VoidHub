import { Post } from "src/shared/models/post-model";
import { Topic } from "./topic.model";
import { User } from "src/shared/models/user.model";

export interface TopicInfo {
  topic: Topic;
  post: Post;
  sender: User;
}
