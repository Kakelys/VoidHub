import { User } from 'src/shared/models/user.model'

export type GeneralStats = {
    topicCount: number
    postCount: number
    userCount: number
    lastUser: User | null
}
