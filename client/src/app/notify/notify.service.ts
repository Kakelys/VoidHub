import { HubConnection } from '@microsoft/signalr'
import { ReplaySubject, Subject, takeUntil } from 'rxjs'

import { Injectable, OnDestroy } from '@angular/core'

import { SignalrService } from 'src/shared/signalr.service'

import { NotificationBase } from './models/notification-base.model'

@Injectable()
export class NotifyService implements OnDestroy {
    private bindings: Map<string, Subject<NotificationBase>> = new Map()
    private destroy$ = new ReplaySubject<boolean>()

    constructor(private signalR: SignalrService) {
        this.signalR.connected$.pipe(takeUntil(this.destroy$)).subscribe((val) => {
            if (val) this.startReceivingNotifies(this.signalR.hub)
        })
    }

    ngOnDestroy(): void {
        this.destroy$.next(true)
        this.destroy$.complete()
    }

    private startReceivingNotifies(hub: HubConnection) {
        hub.on('notify', (ntf: NotificationBase) => {
            if (!this.bindings.has(ntf.type)) {
                console.log('notify', ntf)
                return
            }

            this.bindings.get(ntf.type).next(ntf)
        })
    }

    getSubject(name: string): Subject<NotificationBase> {
        if (!this.bindings.has(name)) this.bindings.set(name, new Subject<NotificationBase>())

        return this.bindings.get(name)
    }
}
