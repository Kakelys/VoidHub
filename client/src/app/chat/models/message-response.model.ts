import { User } from "src/shared/models/user.model";
import { Message } from "./message.model";

export interface MessageResponse {
  message: Message;
  sender: User;
}
