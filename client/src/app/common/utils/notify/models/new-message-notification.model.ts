import { User } from 'src/app/modules/auth'
import { Chat, Message } from 'src/app/modules/chat'

import { NotificationBase } from './notification-base.model'

export type NewMessageNotification = NotificationBase & {
    message: Message
    sender: User
    chat: Chat
    anotherUser: User
}
