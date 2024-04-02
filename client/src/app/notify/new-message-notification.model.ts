import { User } from 'src/shared/models/user.model';
import { Message } from '../chat/models/message.model';
import { NotificationBase } from './notifcation-base.model';
import { Chat } from '../chat/models/chat-model';

export interface NewMessageNotification extends NotificationBase {
  message: Message;
  sender: User;
  chat: Chat;
  anotherUser: User;
}
