import { TranslateService } from '@ngx-translate/core'
import { ToastrService } from 'ngx-toastr'

import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core'
import { NgForm } from '@angular/forms'

import { NgFormExtension } from 'src/shared'

import { HttpException } from 'src/app/common/models'

import { AdminService, BanService } from '../../services'

@Component({
    selector: 'app-ban-menu',
    templateUrl: './ban-menu.component.html',
    styleUrls: ['./ban-menu.component.css'],
})
export class BanMenuComponent implements OnInit {
    @Input()
    username = ''

    userIdBlocked = false

    private _currentDate: Date = new Date()

    public get currentDate(): Date {
        return this._currentDate
    }

    public set currentDate(value: Date) {
        this._currentDate = null
        setTimeout((_) => {
            this._currentDate = value
        })
    }

    @Output()
    banned = new EventEmitter<void>()

    errorMessages = []

    constructor(
        private adminService: AdminService,
        private banService: BanService,
        private toastr: ToastrService,
        private trans: TranslateService
    ) {}

    ngOnInit(): void {
        this.userIdBlocked = this.adminService.userIdBlocked
        this.username = this.adminService.user?.username ?? ''
    }

    onBanSubmit(form: NgForm) {
        this.errorMessages = []

        if (form.invalid) {
            NgFormExtension.markAllAsTouched(form)
            return
        }
        //get ban time
        const banTime = new Date(form.value.expiresAt).getTime() - new Date().getTime()

        if (banTime <= 0) {
            this.trans.get('forms-errors.ban-time').subscribe((str) => {
                this.toastr.error(str)
            })

            return
        }

        //set expiresAt as UTC
        const data = form.value
        const currentUtc = new Date().toISOString()

        data.expiresAt = new Date(
            new Date(currentUtc).getTime() + new Date(banTime).getTime()
        ).toISOString()

        this.banService.banUser(data).subscribe({
            next: (_) => {
                //set values to default
                form.controls['reason'].setValue('')
                form.controls['reason'].setErrors(null)
                this.currentDate = new Date()

                this.trans.get('labels.user-banned').subscribe((str) => {
                    this.toastr.success(str)
                })
            },
            error: (err: HttpException) => {
                this.errorMessages = err.errors
            },
        })
    }

    onUnbanSubmit(form: NgForm) {
        this.errorMessages = []

        if (form.controls['username'].invalid) {
            return
        }

        this.banService.unbanUser(form.value.username).subscribe({
            next: (_) => {
                this.currentDate = new Date()
                this.trans.get('labels.user-unbanned').subscribe((str) => {
                    this.toastr.success(str)
                })
            },
            error: (errs) => {
                this.errorMessages = errs
            },
        })
    }

    onBanClick() {
        this.banned.emit()
    }

    onCancelClick() {
        this.adminService.cancelClicked.next()
    }
}
