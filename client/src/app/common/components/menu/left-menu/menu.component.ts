import { Component, OnInit, OnDestroy } from '@angular/core'
import { ReplaySubject, takeUntil } from 'rxjs'
import { User, AuthService } from 'src/app/modules/auth'
import { SectionResponse, SectionService } from 'src/app/modules/forum'
import { Roles } from 'src/shared'

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
