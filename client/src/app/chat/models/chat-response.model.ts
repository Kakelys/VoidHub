import { User } from 'src/shared/models/user.model'

import { Chat } from './chat-model'
import { Message } from './message.model'

export type ChatResponse = {
    chat: Chat
    lastMessage: Message
    sender: User
    anotherUser: User
}
