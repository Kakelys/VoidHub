import { ReplaySubject, takeUntil } from 'rxjs'

import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit } from '@angular/core'
import { LimiterService } from 'src/app/common'

@Component({
    selector: 'app-limit-loader',
    templateUrl: './limit-loader.component.html',
    styleUrls: ['./limit-loader.component.css'],
})
export class LimitLoaderComponent implements OnDestroy, OnInit {
    @Input()
    limit = -1
    activeRequests = 0

    @Input()
    containerClasses: string = null

    @Input()
    reqName: string

    @Input()
    ignore = false

    private readonly destroy$ = new ReplaySubject<boolean>(1)

    constructor(
        private limiter: LimiterService,
        private cdref: ChangeDetectorRef
    ) {}

    ngOnInit(): void {
        if (!this.reqName) this.reqName = this.limiter.defaultName

        // avoiding errors and log if forgot to add name
        this.limiter.addEmptyIfNotExist(this.reqName)

        this.limiter.nameMap
            .get(this.reqName)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: (activeRequests) => {
                    this.activeRequests = activeRequests
                    this.cdref.detectChanges()
                },
            })
    }

    ngOnDestroy(): void {
        this.destroy$.next(true)
        this.destroy$.complete()
    }
}
