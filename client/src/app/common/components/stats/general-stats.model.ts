import { User } from 'src/app/modules/auth'

export type GeneralStats = {
    topicCount: number
    postCount: number
    userCount: number
    lastUser: User | null
}
