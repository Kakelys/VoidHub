import { User } from '../../auth'

export type LastTopic = {
    id: number
    title: string
    updatedAt: Date
    user: User
}
