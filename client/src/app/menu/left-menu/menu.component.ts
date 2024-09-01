import { ReplaySubject, takeUntil } from 'rxjs'

import { Component, OnDestroy, OnInit } from '@angular/core'

import { User } from 'src/shared/models/user.model'
import { Roles } from 'src/shared/roles.enum'

import { AuthService } from '../../auth/auth.service'
import { SectionResponse } from '../../forum/models/section-response.model'
import { SectionService } from '../../forum/services/section.service'

@Component({
    selector: 'app-menu',
    templateUrl: './menu.component.html',
    styleUrls: ['./menu.component.css'],
})
export class MenuComponent implements OnInit, OnDestroy {
    user: User
    roles = Roles

    sections: SectionResponse[] = []

    private destroy$ = new ReplaySubject<boolean>(1)

    constructor(
        private sectionService: SectionService,
        auth: AuthService
    ) {
        auth.user$.pipe(takeUntil(this.destroy$)).subscribe((user) => {
            this.user = user
        })
    }

    ngOnInit(): void {
        this.sectionService.getShortSections().subscribe({
            next: (data: SectionResponse[]) => {
                this.sections.push(...data)
            },
        })
    }

    ngOnDestroy(): void {
        this.destroy$.next(true)
        this.destroy$.complete()
    }
}
