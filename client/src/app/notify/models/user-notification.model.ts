import { User } from "src/shared/models/user.model";
import { NotificationBase } from "./notifcation-base.model";

export interface UserNotification extends NotificationBase {
  user: User;
}
