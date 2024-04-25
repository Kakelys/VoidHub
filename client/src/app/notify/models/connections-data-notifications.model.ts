import { User } from "src/shared/models/user.model";
import { NotificationBase } from "./notifcation-base.model";

export interface ConnectionsDataNotification extends NotificationBase {
  totalCount: number;
  users: User[];
}
