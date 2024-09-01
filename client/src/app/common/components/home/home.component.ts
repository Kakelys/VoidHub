import { Component } from '@angular/core'
import { takeUntilDestroyed } from '@angular/core/rxjs-interop'
import { User, AuthService } from 'src/app/modules/auth'

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
