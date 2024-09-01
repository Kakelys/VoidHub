import { User } from 'src/shared/models/user.model'

import { NotificationBase } from './notification-base.model'

export type ConnectionsDataNotification = NotificationBase & {
    totalCount: number
    users: User[]
}
