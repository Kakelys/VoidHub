import { User } from "src/shared/models/user.model";

export interface Message {
  id: number,
  content: string,
  createdAt: Date,
  modifiedAt: Date | null
}
