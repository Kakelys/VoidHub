import { User } from 'src/shared/models/user.model'
import { Jwt } from './jwt.model'

export type AuthResponse = {
    tokens: Jwt
    user: User
}
