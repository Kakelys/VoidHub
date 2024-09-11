import { ToastrService } from 'ngx-toastr'

import { Component } from '@angular/core'
import { NgForm } from '@angular/forms'

import { NgFormExtension } from 'src/shared'

import { HttpException } from 'src/app/common/models'
import { ToastrExtension } from 'src/app/common/utils'

import { environment } from 'src/environments/environment'

import { RecoverService } from '../../services'

@Component({
    selector: 'app-password-recover-request',
    templateUrl: './password-recover-request.component.html',
    styleUrls: ['./password-recover-request.component.css'],
})
export class PasswordRecoverRequestComponent {
    isSended = false
    limitNames = environment.limitNames

    constructor(
        private recoverService: RecoverService,
        private toastr: ToastrService
    ) {}

    onSubmit(form: NgForm) {
        if (form.invalid) {
            NgFormExtension.markAllAsTouched(form)
            return
        }

        this.recoverService.sendEmail(form.value.loginOrEmail).subscribe({
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
            complete: () => {
                this.isSended = true
            },
        })
    }
}
