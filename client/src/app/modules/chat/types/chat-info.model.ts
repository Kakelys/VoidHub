import { User } from '../../auth'
import { Chat } from './chat-model'

export type ChatInfo = {
    chat: Chat
    members: User[]
}
