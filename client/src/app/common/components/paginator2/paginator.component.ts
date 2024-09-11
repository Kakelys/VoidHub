import { CommonModule } from '@angular/common'
import { Component, Input, OnChanges, OnInit } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'

const defaultPageSize = 10

@Component({
    selector: 'app-paginator-v2',
    templateUrl: './paginator.component.html',
    styleUrls: ['./paginator.component.css'],
    standalone: true,
    imports: [CommonModule],
})
export class PaginatorV2Component implements OnInit, OnChanges {
    @Input()
    range = 3

    @Input()
    total = 0

    @Input()
    pageSize = defaultPageSize

    @Input()
    queryParamName = 'page'

    currentPage = 1

    minInRange = 1
    maxInRange = 1
    max = 1

    pages: number[] = []

    constructor(
        private router: Router,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.route.queryParams.subscribe((params) => {
            const queryPage = params[this.queryParamName]
            this.currentPage = +queryPage ?? 1

            this.init()

            if (!this.route.snapshot.queryParams[this.queryParamName]) {
                this.onPageClick(1)
            }
        })
    }

    ngOnChanges(): void {
        this.init()
    }

    init() {
        this.max = Math.ceil(this.total / this.pageSize)

        this.updatePages()
    }

    onPageClick(page: number) {
        console.log(page)
        if (page == this.currentPage) {
            return
        }

        if (page < 1) {
            page = 1
        }
        if (page > this.max) {
            page = this.max
        }

        const queryParams = {}
        queryParams[this.queryParamName] = page

        this.router.navigate([], {
            relativeTo: this.route,
            queryParams,
            queryParamsHandling: 'merge',
        })
    }

    updatePages() {
        this.pages = []

        this.minInRange = this.currentPage - this.range
        if (this.minInRange < 1) {
            this.minInRange = 1
        }
        this.maxInRange = this.currentPage + this.range
        if (this.maxInRange > this.max) {
            this.maxInRange = this.max
        }

        for (let i = this.minInRange; i <= this.maxInRange; i++) {
            this.pages.push(i)
        }
    }
}
