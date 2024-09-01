import { User } from '../../auth/types/user.model'

export type PostEdit = {
    id: number
    content: string
    createdAt: Date
    user: User
}
