import { Component, OnInit } from '@angular/core'
import { ActivatedRoute } from '@angular/router'
import { ToastrService } from 'ngx-toastr'
import { take } from 'rxjs'
import { HttpException, ToastrExtension } from 'src/app/common'
import { AuthService } from 'src/app/modules/auth'

import { environment } from 'src/environments/environment'
import { EmailService } from '../../services'

@Component({
    selector: 'app-confirm-email',
    templateUrl: './confirm-email.component.html',
    styleUrls: ['./confirm-email.component.css'],
})
export class ConfirmEmailComponent implements OnInit {
    confirmed = false

    limitNames = environment.limitNames

    constructor(
        private emailService: EmailService,
        private route: ActivatedRoute,
        private toastr: ToastrService,
        private auth: AuthService
    ) {}

    ngOnInit(): void {
        this.emailService.confirmEmail(this.route.snapshot.params['token']).subscribe({
            next: (_) => {
                this.confirmed = true

                this.auth.user$.pipe(take(1)).subscribe((user) => {
                    if (!user) return

                    user.isEmailConfirmed = true
                    this.auth.updateUser(user)
                })
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }
}
