import { Component } from '@angular/core'
import { NgForm } from '@angular/forms'
import { Router } from '@angular/router'

import { NgFormExtension } from 'src/shared'

import { HttpException } from 'src/app/common/models'

import { AuthService } from '../../services'

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css'],
})
export class LoginComponent {
    errorMessages: string[] = []

    constructor(
        private authService: AuthService,
        private router: Router
    ) {}

    onSubmit(form: NgForm) {
        this.errorMessages = []

        if (form.invalid) {
            NgFormExtension.markAllAsTouched(form)
            return
        }

        this.authService.login(form.value).subscribe({
            next: (_) => {
                this.router.navigate(['/'])
            },
            error: (err: HttpException) => {
                this.errorMessages = err.errors
            },
        })
    }
}
