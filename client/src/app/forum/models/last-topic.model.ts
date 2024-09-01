import { User } from 'src/shared/models/user.model'

export type LastTopic = {
    id: number
    title: string
    updatedAt: Date
    user: User
}
