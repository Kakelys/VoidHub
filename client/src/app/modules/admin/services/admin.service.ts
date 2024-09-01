import { Subject } from 'rxjs'

import { Injectable } from '@angular/core'

import { User } from '../../auth'

@Injectable()
export class AdminService {
    cancelClicked = new Subject<void>()
    public user: User | null = null
    public userIdBlocked: boolean
}
