import { ReplaySubject, takeUntil } from 'rxjs'

import { Component, OnDestroy } from '@angular/core'

import { User } from 'src/shared/models/user.model'
import { Roles } from 'src/shared/roles.enum'

import { SearchParams } from '../forum/models/search-params.model'
import { AuthService } from './../auth/auth.service'

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.css'],
})
export class HeaderComponent implements OnDestroy {
    private readonly destroy$ = new ReplaySubject<boolean>(1)
    defaultSearch: SearchParams = {
        sort: '',
        forumId: 0,
        withPostContent: false,
        onlyDeleted: false,
        partialTitle: true,
    }

    user: User = null
    roles = Roles

    constructor(private authService: AuthService) {
        authService.user$.pipe(takeUntil(this.destroy$)).subscribe({
            next: (data) => {
                this.user = data
            },
        })
    }

    logout() {
        this.authService.logout()
    }

    ngOnDestroy(): void {
        this.destroy$.next(true)
        this.destroy$.complete()
    }
}
