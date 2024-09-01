import { ToastrService } from 'ngx-toastr'
import { ReplaySubject, takeUntil } from 'rxjs'

import { Component, OnDestroy } from '@angular/core'
import { NgForm } from '@angular/forms'
import { ActivatedRoute, Router } from '@angular/router'

import { HttpException } from 'src/shared/models/http-exception.model'
import { User } from 'src/shared/models/user.model'
import { NgFormExtension } from 'src/shared/ng-form.extension'
import { Roles } from 'src/shared/roles.enum'
import { ToastrExtension } from 'src/shared/toastr.extension'

import { AuthService } from 'src/app/auth/auth.service'
import { Chat } from 'src/app/chat/models/chat-model'
import { ChatService } from 'src/app/chat/services/chat.service'
import { AdminService } from 'src/app/forum/admin/services/admin.service'

import { environment } from 'src/environments/environment'

import { AccountService } from '../account.service'

@Component({
    selector: 'app-profile',
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.css'],
})
export class ProfileComponent implements OnDestroy {
    resourceUrl = environment.resourceURL

    daysBetween = 0

    user: User = null
    profile: any = null
    userId = null
    roles = Roles
    pChat: Chat = null

    private destroy$ = new ReplaySubject<boolean>(1)

    constructor(
        private accountService: AccountService,
        private route: ActivatedRoute,
        private adminService: AdminService,
        private chatService: ChatService,
        private toastr: ToastrService,
        private router: Router,
        authService: AuthService
    ) {
        authService.user$.pipe(takeUntil(this.destroy$)).subscribe((user) => {
            this.user = user
        })

        adminService.cancelClicked.pipe(takeUntil(this.destroy$)).subscribe((_) => {
            router.navigate(['../'], { relativeTo: this.route })
        })

        route.params.subscribe(async (params) => {
            this.handleIdChange(params['id'])
        })
    }

    async handleIdChange(newId: number) {
        if (this.userId == newId) return

        this.userId = newId

        this.accountService.getAccount(this.userId).subscribe({
            next: (user: any) => {
                this.adminService.user = user
                this.profile = user

                // days between register day and today
                const oneDay = 1000 * 60 * 60 * 24
                const diffInTime = new Date().getTime() - new Date(user.createdAt).getTime()
                this.daysBetween = Math.round(diffInTime / oneDay)

                if (this.user) this.updateChatBetween()
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }

    updateChatBetween() {
        this.chatService.getChatBetween(this.profile.id).subscribe({
            next: (chat: Chat | null) => {
                this.pChat = chat
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }

    onStartChat(form: NgForm) {
        if (form.invalid) {
            NgFormExtension.markAllAsTouched(form)
            return
        }

        this.chatService.createPersonal(form.value.content, this.profile.id).subscribe({
            next: (chat: Chat) => {
                this.router.navigate(['/chats/', chat.id])
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }

    ngOnDestroy(): void {
        this.destroy$.next(true)
        this.destroy$.complete()
        this.adminService.user = null
    }
}
