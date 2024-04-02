import { User } from "src/shared/models/user.model";
import { Chat } from "./chat-model";

export interface ChatInfo {
  chat: Chat;
  members: User[]
}
