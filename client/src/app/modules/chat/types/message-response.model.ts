import { User } from '../../auth'
import { Message } from './message.model'

export type MessageResponse = {
    message: Message
    sender: User
}
