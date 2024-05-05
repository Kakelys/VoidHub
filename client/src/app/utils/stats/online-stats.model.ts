import { User } from "src/shared/models/user.model";

export interface OnlineStats {
  anonimCount: number;
  users: User[];
}
