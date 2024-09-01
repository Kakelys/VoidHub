import { User } from 'src/shared/models/user.model'

import { Chat } from '../../chat/models/chat-model'
import { Message } from '../../chat/models/message.model'
import { NotificationBase } from './notification-base.model'

export type NewMessageNotification = NotificationBase & {
    message: Message
    sender: User
    chat: Chat
    anotherUser: User
}
