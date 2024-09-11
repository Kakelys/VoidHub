import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr'
import { BehaviorSubject, ReplaySubject, takeUntil } from 'rxjs'

import { Injectable, OnDestroy } from '@angular/core'

import { AuthService } from 'src/app/modules/auth'

import { environment as env } from 'src/environments/environment'

@Injectable()
export class SignalrService implements OnDestroy {
    connected$ = new BehaviorSubject<boolean>(false)
    hub: HubConnection

    private destroy$ = new ReplaySubject(1)

    constructor(private auth: AuthService) {
        // to be sure, that all startup auth handlers completed
        setTimeout((_) => {
            this.auth.user$.pipe(takeUntil(this.destroy$)).subscribe((user) => {
                this.start()
            })
        }, 1000)
    }

    ngOnDestroy(): void {
        this.destroy$.next(true)
        this.destroy$.complete()
    }

    public async start() {
        if (this.hub) await this.stop()

        this.hub = new HubConnectionBuilder()
            .withUrl(env.baseAPIUrl + '/v1/signalr', {
                accessTokenFactory: () => localStorage.getItem('access-token'),
                withCredentials: false,
            })
            .withAutomaticReconnect()
            .build()

        this.hub
            .start()
            .then((_) => {
                console.log('signalR: connected')
                this.connected$.next(true)
            })
            .catch((err) => {
                console.log('signalR error ', err)
            })
    }

    public async stop() {
        if (this.hub && this.hub.state != HubConnectionState.Disconnected) {
            console.log('signalR: disconnected')
            await this.hub.stop()
            this.connected$.next(false)
        }
    }
}
