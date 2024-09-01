import { Component, OnDestroy } from '@angular/core'
import { NgForm } from '@angular/forms'
import { ReplaySubject, take, takeUntil } from 'rxjs'
import { AuthService } from 'src/app/auth/auth.service'
import { User } from 'src/shared/models/user.model'
import { NgFormExtension } from 'src/shared/ng-form.extension'
import { AccountService } from '../account.service'
import { ToastrService } from 'ngx-toastr'
import { environment as env } from 'src/environments/environment'
import { HttpEvent } from '@angular/common/http'
import { HttpException } from 'src/shared/models/http-exception.model'
import { Router } from '@angular/router'
import { EmailService } from '../email.service'
import { TranslateService } from '@ngx-translate/core'

@Component({
    selector: 'app-settings',
    templateUrl: './settings.component.html',
    styleUrls: ['./settings.component.css'],
})
export class SettingsComponent implements OnDestroy {
    errorMessages: string[] = []

    avatarUrl: string = null
    urlDynamicParam: string = Date.now().toString()
    fileToUpload: File | null = null
    fileUploading = false
    avatarProgress = 0

    user: User
    private destroy$ = new ReplaySubject<boolean>(1)

    constructor(
        private authService: AuthService,
        private accountService: AccountService,
        private emailService: EmailService,
        private toastr: ToastrService,
        private router: Router,
        private trans: TranslateService
    ) {
        authService.user$.pipe(takeUntil(this.destroy$)).subscribe((user) => {
            if (!user) {
                this.router.navigate(['/'])
                return
            }

            this.user = user
            this.urlDynamicParam = Date.now().toString()
            this.avatarUrl = `${env.resourceURL}/${user.avatarPath}`
        })
    }

    onProfileSubmit(form: NgForm) {
        this.errorMessages = []
        const data = form.value

        if ((data.oldPassword && !data.newPassword) || (!data.oldPassword && data.newPassword)) {
            this.trans.get('forms-errors.old-and-new-required').subscribe((str) => {
                this.errorMessages.push(str)
            })
            return
        }

        if (form.invalid || (data.newPassword && data.newPassword.length < 8)) {
            NgFormExtension.markAllAsTouched(form)
            return
        }

        this.accountService.updAccount(data).subscribe({
            next: (user: User) => {
                this.trans.get('labels.account-updated-succ').subscribe((str) => {
                    this.toastr.success(str)
                })
                this.authService.updateUser(user)
            },
            error: (err: HttpException) => {
                this.errorMessages = err.errors
            },
        })
    }

    handleFileInput(target: any) {
        this.fileToUpload = target.files[0]
    }

    onAvatarSubmit(form: NgForm) {
        this.errorMessages = []

        if (form.invalid) {
            NgFormExtension.markAllAsTouched(form)
            return
        }

        if (!this.fileToUpload) {
            this.trans.get('forms-errors.file-required').subscribe((str) => {
                this.errorMessages.push(`${str}`)
            })
            return
        }

        if (this.fileToUpload.size > env.maxAvatarSize) {
            this.trans.get('forms-errors.file-size').subscribe((str) => {
                this.errorMessages.push(`${str} ${env.maxAvatarSize / 1024} KB`)
            })

            return
        }

        this.fileUploading = true

        const formData = new FormData()
        formData.append('img', this.fileToUpload)
        this.accountService.updAvatar(formData, this.user.avatarPath).subscribe({
            next: (event: HttpEvent<User>) => {
                if (event.type == 1) {
                    this.avatarProgress = Math.round((100 * event.loaded) / event.total)
                }
                if (event.type == 4) {
                    if (event.body) {
                        this.authService.updateUser(event.body)
                    } else {
                        this.urlDynamicParam = Date.now().toString()
                    }

                    this.trans.get('labels.avatar-updated-successfully').subscribe((str) => {
                        this.toastr.success(str)
                    })

                    this.fileToUpload = null
                    form.reset()
                }
            },
            error: (err: HttpException) => {
                this.errorMessages = err.errors
            },
            complete: () => {
                this.fileUploading = false
            },
        })
    }

    onResendClick() {
        this.emailService.resendConfirmEmail().subscribe({
            next: (_) => {
                this.trans.get('labels.done-check-email').subscribe((str) => {
                    this.toastr.success(str)
                })
            },
            error: (err: HttpException) => {
                this.errorMessages = err.errors
            },
        })
    }

    ngOnDestroy(): void {
        this.destroy$.next(true)
        this.destroy$.complete()
    }
}
