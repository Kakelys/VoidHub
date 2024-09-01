import { User } from 'src/shared/models/user.model'

import { NotificationBase } from './notification-base.model'

export type UserNotification = NotificationBase & {
    user: User
}
