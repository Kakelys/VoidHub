import { Post } from "src/shared/models/post-model";
import { Topic } from "./topic.model";
import { User } from "src/shared/models/user.model";
import { Forum } from "./forum.model";

export interface TopicInfo {
  topic: Topic;
  forum: Forum;
  post: Post;
  sender: User;
}
