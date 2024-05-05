import { User } from "src/shared/models/user.model";

export interface GeneralStats {
  topicCount: number;
  postCount: number;
  userCount: number;
  lastUser: User | null;
}
