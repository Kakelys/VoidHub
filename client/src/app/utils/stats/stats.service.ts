import { BehaviorSubject, debounceTime } from 'rxjs'

import { HttpClient } from '@angular/common/http'
import { Injectable } from '@angular/core'
import { takeUntilDestroyed } from '@angular/core/rxjs-interop'

import { SignalrService } from 'src/shared/signalr.service'

import { ConnectionsDataNotification } from 'src/app/notify/models/connections-data-notifications.model'
import { NotifyService } from 'src/app/notify/notify.service'

import { environment } from 'src/environments/environment'

import { GeneralStats } from './general-stats.model'
import { OnlineStats } from './online-stats.model'

@Injectable()
export class StatsService {
    private baseUrl = environment.baseAPIUrl + '/v1/stats/'

    onlineStats$ = new BehaviorSubject<OnlineStats>({ anonimCount: 0, users: [] })
    generalStats$ = new BehaviorSubject<GeneralStats>({
        topicCount: 0,
        postCount: 0,
        userCount: 0,
        lastUser: null,
    })

    constructor(
        private http: HttpClient,
        signalR: SignalrService,
        notifyService: NotifyService
    ) {
        notifyService
            .getSubject('connectionsData')
            .pipe(takeUntilDestroyed())
            .subscribe((notif: ConnectionsDataNotification) => this.handleConnectionsData(notif))

        signalR.connected$.pipe(takeUntilDestroyed(), debounceTime(1000)).subscribe((val) => {
            if (val) {
                signalR.hub.invoke('GetConnectionsData')
            }
        })

        this.getGeneral().subscribe({
            next: (data: GeneralStats) => {
                this.generalStats$.next(data)
            },
            error: (err) => {
                console.error(err)
            },
        })
    }

    getGeneral() {
        return this.http.get(this.baseUrl + 'general')
    }

    getOnline() {
        return this.http.get(this.baseUrl + 'online')
    }

    private handleConnectionsData(notif: ConnectionsDataNotification) {
        this.onlineStats$.next({
            users: notif.users,
            anonimCount: notif.totalCount - notif.users.length,
        })
    }
}
