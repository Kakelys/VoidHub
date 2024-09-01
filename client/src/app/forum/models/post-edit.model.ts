import { User } from '../../../shared/models/user.model'

export type PostEdit = {
    id: number
    content: string
    createdAt: Date
    user: User
}
