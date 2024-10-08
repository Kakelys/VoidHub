import { TranslateService } from '@ngx-translate/core'
import { ToastrService } from 'ngx-toastr'

import { Component, Input, OnInit } from '@angular/core'
import { NgForm } from '@angular/forms'

import { NgFormExtension, Roles } from 'src/shared'

import { HttpException } from 'src/app/common/models'
import { AccountService } from 'src/app/modules/forum'

import { AdminService } from '../../services'

@Component({
    selector: 'app-role-menu',
    templateUrl: './role-menu.component.html',
    styleUrls: ['./role-menu.component.css'],
})
export class RoleMenuComponent implements OnInit {
    @Input()
    username: string

    userIdBlocked

    roles = Object.keys(Roles).map((key) => Roles[key])

    errorMessages = []

    constructor(
        private adminService: AdminService,
        private accountService: AccountService,
        private toastr: ToastrService,
        private trans: TranslateService
    ) {}

    ngOnInit(): void {
        this.userIdBlocked = this.adminService.userIdBlocked
        this.username = this.adminService.user?.username ?? ''
    }

    onSubmit(form: NgForm) {
        this.errorMessages = []

        if (this.roles.indexOf(form.value.roleName) === -1)
            form.controls['roleName'].setErrors({ incorrect: true })

        if (form.invalid) {
            NgFormExtension.markAllAsTouched(form)
            return
        }

        this.accountService.updateRole(form.value.username, form.value.roleName).subscribe({
            next: () => {
                this.trans.get('labels.role-updated-successfully').subscribe((str) => {
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
