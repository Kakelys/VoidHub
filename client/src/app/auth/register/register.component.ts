import { Component } from '@angular/core'
import { NgForm } from '@angular/forms'
import { Router } from '@angular/router'

import { HttpException } from 'src/shared/models/http-exception.model'
import { NgFormExtension } from 'src/shared/ng-form.extension'

import { AuthService } from './../auth.service'

@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.css'],
})
export class RegisterComponent {
    errorMessages: string[] = []

    constructor(
        private authService: AuthService,
        private router: Router
    ) {}

    onSubmit(form: NgForm) {
        this.errorMessages = []
        const data = form.value

        if (form.invalid || data.password != data.confirmPassword) {
            NgFormExtension.markAllAsTouched(form)
            return
        }

        this.authService.register(data).subscribe({
            next: (_) => {
                this.router.navigate(['/'])
            },
            error: (err: HttpException) => {
                this.errorMessages = err.errors
            },
        })
    }
}
