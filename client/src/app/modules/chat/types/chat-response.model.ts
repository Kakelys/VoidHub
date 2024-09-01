import { User } from '../../auth'
import { Chat } from './chat-model'
import { Message } from './message.model'

export type ChatResponse = {
    chat: Chat
    lastMessage: Message
    sender: User
    anotherUser: User
}
