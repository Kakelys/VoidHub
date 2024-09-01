import { ToastrService } from 'ngx-toastr'
import { ReplaySubject, Subscription, takeUntil } from 'rxjs'

import { Injectable, OnDestroy } from '@angular/core'
import { Router } from '@angular/router'

import { CustomSound } from 'src/shared/sound.extension'

import { AuthService } from '../../auth/auth.service'
import { NewMessageNotification } from '../models/new-message-notification.model'
import { NotifyService } from '../notify.service'
import { NewMessageComponent } from './new-message.component'

@Injectable()
export class NewMessageListener implements OnDestroy {
    private ignoredId: number

    private destroy$ = new ReplaySubject()

    private sub: Subscription

    constructor(
        private notifyService: NotifyService,
        private toastr: ToastrService,
        private router: Router,
        private auth: AuthService
    ) {
        this.auth.user$.pipe(takeUntil(this.destroy$)).subscribe((user) => {
            if (user) {
                this.ignoredId = user.id
                this.start()
            } else {
                this.stop()
            }
        })
    }

    ngOnDestroy(): void {
        this.destroy$.next(true)
        this.destroy$.complete()
    }

    public async start() {
        await this.stop()

        this.sub = this.notifyService
            .getSubject('newMessage')
            .pipe(takeUntil(this.destroy$))
            .subscribe((notify: NewMessageNotification) => {
                this.onNewMessage(notify)
            })
    }

    public async stop() {
        if (!this.sub) return

        this.sub.unsubscribe()
    }

    private onNewMessage(notify: NewMessageNotification) {
        if (this.router.url.indexOf('chats') != -1 || notify.sender.id == this.ignoredId) return

        this.toastr.success(notify.message.content, notify.sender.username, {
            tapToDismiss: true,
            toastComponent: NewMessageComponent,
            toastClass: '',
            positionClass: 'toast-bottom-right',
        })

        CustomSound.playMetalPipe()
    }
}
