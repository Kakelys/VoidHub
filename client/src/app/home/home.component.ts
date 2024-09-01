import { Component } from '@angular/core'
import { takeUntilDestroyed } from '@angular/core/rxjs-interop'

import { User } from 'src/shared/models/user.model'

import { AuthService } from '../auth/auth.service'

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css'],
})
export class HomeComponent {
    user: User

    constructor(authService: AuthService) {
        authService.user$.pipe(takeUntilDestroyed()).subscribe((user) => {
            this.user = user
        })
    }
}
