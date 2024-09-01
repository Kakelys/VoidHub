import { Component } from '@angular/core'
import { NgForm } from '@angular/forms'
import { Router } from '@angular/router'

import { HttpException } from 'src/shared/models/http-exception.model'
import { NgFormExtension } from 'src/shared/ng-form.extension'

import { AuthService } from '../auth.service'

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
