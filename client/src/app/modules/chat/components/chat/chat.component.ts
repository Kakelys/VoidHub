import { TranslateService } from '@ngx-translate/core'
import { ToastrService } from 'ngx-toastr'
import { ReplaySubject, debounceTime, fromEvent, take, takeUntil } from 'rxjs'

import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core'
import { NgForm } from '@angular/forms'
import { ActivatedRoute, Router } from '@angular/router'

import { NgFormExtension, Offset } from 'src/shared'

import { HttpException } from 'src/app/common/models'
import { NotifyService, ToastrExtension } from 'src/app/common/utils'
import { NewMessageNotification } from 'src/app/common/utils/notify/models/new-message-notification.model'
import { AuthService, User } from 'src/app/modules/auth'

import { environment as env } from 'src/environments/environment'

import { ChatService } from '../../services'
import { ChatInfo, MessageResponse } from '../../types'

@Component({
    selector: 'app-chat',
    templateUrl: './chat.component.html',
    styleUrls: ['./chat.component.css'],
})
export class ChatComponent implements OnInit, OnDestroy {
    user: User

    chatId: number
    chat: ChatInfo
    messages: MessageResponse[] = []

    chatName: string
    profileId: number

    loadTime: Date = new Date()
    firstLoad: Date = new Date()
    canLoadMore = true
    loading = false
    messageLimit = 50

    limitNames = env.limitNames

    message = ''

    private destroy$ = new ReplaySubject<boolean>(1)

    private savedMessageStr = ''

    @ViewChild('messagesContainer', { static: true })
    messagesContainer: ElementRef

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private toastr: ToastrService,
        private chatService: ChatService,
        private auth: AuthService,
        private notifyService: NotifyService,
        private trans: TranslateService
    ) {}

    ngOnInit(): void {
        this.trans
            .get('chat.saved')
            .pipe(take(1))
            .subscribe((str) => {
                this.savedMessageStr = str
            })

        this.route.params.subscribe((params) => {
            this.handleChatId(params['id'])
        })

        this.auth.user$.pipe(takeUntil(this.destroy$)).subscribe((user: User) => {
            this.user = user
        })

        fromEvent(this.messagesContainer.nativeElement, 'scroll')
            .pipe(takeUntil(this.destroy$), debounceTime(300))
            .subscribe((e: any) => {
                const dif = e.target.scrollTop - e.target.clientHeight

                if ((dif + dif * 0.3) * -1 > e.target.scrollHeight) this.loadMessages()
            })

        this.notifyService
            .getSubject('newMessage')
            .pipe(takeUntil(this.destroy$))
            .subscribe((notify: NewMessageNotification) => {
                if (notify.chat.id != this.chatId) return

                console.log(1)
                this.onNewMessage(notify)
            })
    }

    async handleChatId(chatId) {
        const newChatId = chatId

        if (!Number(newChatId)) {
            this.trans.get('chat.wrongid').subscribe((msg) => {
                this.toastr.error(msg)
            })
            this.router.navigate(['/chats'])

            return
        }

        if (newChatId == this.chatId) return

        this.chatId = newChatId
        this.chatService.currentChat$.next(this.chatId)

        this.setDefaults()
        this.loadChatInfo()
    }

    async loadMessages() {
        if (!this.canLoadMore || this.loading) return
        this.loading = true

        const additionalOffset = this.messages.filter(
            (c) => c.message.createdAt > this.loadTime
        ).length
        const offset = new Offset(this.messages.length + additionalOffset, this.messageLimit)

        this.chatService.getMessages(this.chatId, offset, this.loadTime).subscribe({
            next: (msgs: MessageResponse[]) => {
                if (!msgs || msgs.length < this.messageLimit) {
                    this.canLoadMore = false
                }
                const isFirst = this.messages.length == 0

                if (isFirst) {
                    this.messages = msgs
                    setTimeout((_) => {
                        this.messagesContainer.nativeElement.scrollTop = 0
                    }, 100)
                } else this.messages.push(...msgs)
            },
            error: (err: HttpException) => ToastrExtension.handleErrors(this.toastr, err.errors),
            complete: () => {
                this.loading = false
            },
        })
    }

    loadChatInfo() {
        this.chatService.getChat(this.chatId).subscribe({
            next: (chat: ChatInfo) => {
                this.chat = chat
                if (chat.chat.isGroup) {
                    this.chatName = chat.chat.title
                } else {
                    const anotherUser = chat.members.find((m) => m.id != this.user.id)
                    this.profileId = anotherUser?.id ?? this.user.id
                    this.chatName = anotherUser?.username ?? this.savedMessageStr
                }

                this.loadMessages()
            },
            error: (err: HttpException) => ToastrExtension.handleErrors(this.toastr, err.errors),
        })
    }

    onSendMessage(form: NgForm) {
        if (form.invalid) {
            NgFormExtension.markAllAsTouched(form)
            return
        }

        this.chatService.sendMessage(this.chatId, form.value.content).subscribe({
            next: (msg: MessageResponse) => {
                form.reset()
            },
            error: (err: HttpException) => ToastrExtension.handleErrors(this.toastr, err.errors),
        })
    }

    onNewMessage(notify: NewMessageNotification) {
        const msg: MessageResponse = {
            message: notify.message,
            sender: notify.sender,
        }

        this.messages.unshift(msg)
    }

    onKeyDown(event: KeyboardEvent, form: NgForm) {
        if (event.key == 'Enter') {
            event.preventDefault()
            if (event.shiftKey) {
                if (!this.message) this.message = ' '

                this.message += '\n'
            } else {
                this.onSendMessage(form)
                form.reset()
            }
        }
    }

    setDefaults() {
        this.chat = null
        this.messages = []
        this.messagesContainer.nativeElement.scrollTop = 0

        this.canLoadMore = true
        this.loadTime = new Date()
    }

    ngOnDestroy(): void {
        this.destroy$.next(true)
        this.destroy$.complete()

        this.chatService.currentChat$.next(null)
    }
}
