import { TranslateService } from '@ngx-translate/core'
import { ToastrService } from 'ngx-toastr'

import { Component, Input, OnInit } from '@angular/core'
import { NgForm } from '@angular/forms'

import { NgFormExtension } from 'src/shared'

import { HttpException } from 'src/app/common/models'
import { AccountService } from 'src/app/modules/forum'

import { AdminService } from '../../services'

@Component({
    selector: 'app-rename-menu',
    templateUrl: './rename-menu.component.html',
    styleUrls: ['./rename-menu.component.css'],
})
export class RenameMenuComponent implements OnInit {
    @Input()
    currentUsername: string

    @Input()
    username

    userIdBlocked
    errorMessages = []

    constructor(
        private adminService: AdminService,
        private accountService: AccountService,
        private toastr: ToastrService,
        private trans: TranslateService
    ) {}

    ngOnInit(): void {
        this.userIdBlocked = this.adminService.userIdBlocked

        if (!this.currentUsername) this.currentUsername = this.adminService.user.username

        if (!this.username) this.username = this.adminService.user.username
    }

    onSubmit(form: NgForm) {
        this.errorMessages = []

        if (form.invalid) {
            NgFormExtension.markAllAsTouched(form)
            return
        }

        this.accountService.updateUsername(form.value.currentUsername, form.value).subscribe({
            next: () => {
                this.trans.get('labels.name-updated-successfully').subscribe((str) => {
                    this.toastr.success(str)
                })
            },
            error: (err: HttpException) => {
                this.errorMessages = err.errors
            },
        })
    }

    onCancelClick() {
        this.adminService.cancelClicked.next()
    }
}
