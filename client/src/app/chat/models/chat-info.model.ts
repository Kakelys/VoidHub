import { User } from 'src/shared/models/user.model'

import { Chat } from './chat-model'

export type ChatInfo = {
    chat: Chat
    members: User[]
}
