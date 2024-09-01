import { ToastrService } from 'ngx-toastr'
import { ReplaySubject, takeUntil } from 'rxjs'

import { Component, OnDestroy } from '@angular/core'

import { HttpException } from 'src/shared/models/http-exception.model'
import { User } from 'src/shared/models/user.model'
import { Roles } from 'src/shared/roles.enum'
import { ToastrExtension } from 'src/shared/toastr.extension'

import { AuthService } from 'src/app/auth/auth.service'

import { SectionFullResponse } from '../../models/section-full-response.model'
import { SectionService } from '../../services/section.service'

@Component({
    selector: 'app-section-list',
    templateUrl: './section-list.component.html',
    styleUrls: ['./section-list.component.css'],
})
export class SectionListComponent implements OnDestroy {
    sections: SectionFullResponse[] = []
    roles = Roles

    private destroy$ = new ReplaySubject<boolean>(1)
    user: User = null

    constructor(
        private authService: AuthService,
        private sectionService: SectionService,
        private toastr: ToastrService
    ) {
        authService.user$.pipe(takeUntil(this.destroy$)).subscribe((user) => {
            this.user = user
        })

        sectionService.getSections().subscribe({
            next: (sections: SectionFullResponse[]) => {
                this.sections = sections
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }

    onSectionDeleted(id: number) {
        this.sections = this.sections.filter((s) => s.section.id != id)
    }

    ngOnDestroy(): void {
        this.destroy$.next(true)
        this.destroy$.complete()
    }
}
