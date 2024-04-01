import { Message } from './message.model';
import { Chat } from './chat-model';
import { User } from 'src/shared/models/user.model';

export interface ChatResponse  {
  chat: Chat,
  lastMessage: Message,
  sender: User,
  anotherUser: User
}
