import { Jwt } from './jwt.model'
import { User } from './user.model'

export type AuthResponse = {
    tokens: Jwt
    user: User
}
