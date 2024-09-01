import { ReplaySubject, takeUntil } from 'rxjs'

import { Component, Input, OnDestroy, OnInit } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'

import { Roles } from 'src/shared/roles.enum'

import { AuthService, User } from 'src/app/modules/auth'

import { AdminService } from '../../services'

@Component({
    selector: 'app-admin-panel',
    templateUrl: './admin-panel.component.html',
    styleUrls: ['./admin-panel.component.css'],
})
export class AdminPanelComponent implements OnDestroy, OnInit {
    @Input()
    blockUserId = false

    @Input()
    skipRouting = false

    @Input()
    user: User

    roles = Roles

    private readonly destroy$ = new ReplaySubject<boolean>(1)

    constructor(
        private adminService: AdminService,
        private router: Router,
        private route: ActivatedRoute,
        private authService: AuthService
    ) {
        this.adminService.cancelClicked
            .pipe(takeUntil(this.destroy$))
            .subscribe((_) => this.close())

        this.authService.user$
            .pipe(takeUntil(this.destroy$))
            .subscribe((user) => (this.user = user))
    }

    ngOnInit(): void {
        this.adminService.userIdBlocked = this.blockUserId
    }

    close() {
        const newRoute = this.router.url.substring(0, this.router.url.lastIndexOf('/'))
        this.router.navigate([newRoute])
    }

    ngOnDestroy(): void {
        this.destroy$.next(true)
        this.destroy$.complete()
    }
}
