import { TranslateService } from '@ngx-translate/core'
import { ToastrService } from 'ngx-toastr'

import { Component } from '@angular/core'
import { NgForm } from '@angular/forms'
import { ActivatedRoute, Router } from '@angular/router'

import { NgFormExtension } from 'src/shared'

import { HttpException } from 'src/app/common/models'
import { ToastrExtension } from 'src/app/common/utils'

import { environment } from 'src/environments/environment'

import { RecoverService } from '../../services'

@Component({
    selector: 'app-password-recover',
    templateUrl: './password-recover.component.html',
    styleUrls: ['./password-recover.component.css'],
})
export class PasswordRecoverComponent {
    limitNames = environment.limitNames

    constructor(
        private recoverService: RecoverService,
        private route: ActivatedRoute,
        private router: Router,
        private toastr: ToastrService,
        private trans: TranslateService
    ) {}

    onSubmit(form: NgForm) {
        const data = form.value

        if (form.invalid || data.password != data.confirmPassword) {
            NgFormExtension.markAllAsTouched(form)
            return
        }

        this.recoverService.recover(this.route.snapshot.params['token'], data.password).subscribe({
            next: (_) => {
                this.trans.get('labels.using-new-password').subscribe((str) => {
                    this.toastr.success(str)
                    this.router.navigate(['/login'])
                })
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }
}
