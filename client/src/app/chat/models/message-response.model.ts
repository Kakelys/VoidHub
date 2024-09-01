import { User } from 'src/shared/models/user.model'

import { Message } from './message.model'

export type MessageResponse = {
    message: Message
    sender: User
}
