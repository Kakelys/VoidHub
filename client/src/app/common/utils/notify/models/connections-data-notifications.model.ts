import { User } from 'src/app/modules/auth'

import { NotificationBase } from './notification-base.model'

export type ConnectionsDataNotification = NotificationBase & {
    totalCount: number
    users: User[]
}
