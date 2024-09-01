import { Component } from '@angular/core'
import { takeUntilDestroyed } from '@angular/core/rxjs-interop'
import { ActivatedRoute, Router } from '@angular/router'

import { HttpException } from 'src/shared/models/http-exception.model'
import { User } from 'src/shared/models/user.model'
import { Page } from 'src/shared/page.model'
import { Roles } from 'src/shared/roles.enum'
import { StringExtension } from 'src/shared/string.extension'

import { AuthService } from 'src/app/auth/auth.service'

import { environment } from 'src/environments/environment'

import { SearchElementType } from '../../models/search-element.enum'
import { SearchParams } from '../../models/search-params.model'
import { SearchResponse } from '../../models/search-response.model'
import { SearchService } from '../../services/search.service'
import { SearchSort } from '../search-sort.enum'

@Component({
    selector: 'app-search',
    templateUrl: './search.component.html',
    styleUrls: ['./search.component.css'],
})
export class SearchComponent {
    searchResult: SearchResponse | null

    searchParams: SearchParams
    searchPage: Page = new Page(1)
    query = ''
    resTypes = SearchElementType

    currentPage = 1
    errorMessages = []

    user: User

    resourceUrl = environment.resourceURL
    roles = Roles

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private searchService: SearchService,
        auth: AuthService
    ) {
        auth.user$.pipe(takeUntilDestroyed()).subscribe((user) => {
            this.user = user
        })

        this.activatedRoute.queryParams.subscribe((params) => {
            // mb just use ...params instead
            const newSearchParams: SearchParams = {
                sort: SearchSort.New.toString(),
                forumId: 0,
                withPostContent: false,
                onlyDeleted: false,
                partialTitle: false,
            }

            newSearchParams.sort = params['sort'] ? params['sort'] : SearchSort.New.toString()
            newSearchParams.forumId = params['forumId'] ? params['forumId'] : 0
            newSearchParams.withPostContent =
                StringExtension.ConvertToBoolean(params['withPostContent']) ?? false
            newSearchParams.onlyDeleted =
                StringExtension.ConvertToBoolean(params['onlyDeleted']) ?? false
            newSearchParams.partialTitle =
                StringExtension.ConvertToBoolean(params['partialTitle']) ?? false

            const newSearchPage = new Page(
                +params['pageNumber'] ? +params['pageNumber'] : this.searchPage.pageNumber,
                +params['pageSize'] ? +params['pageSize'] : this.searchPage.pageSize
            )

            const newQuery = params['query']

            const isParamsChanged = this.searchParams + '' != newSearchParams + ''
            const isQueryChanged = this.query != newQuery
            const isPageChanged = !this.searchPage.Equals(newSearchPage)

            this.query = newQuery ?? ''
            this.searchParams = newSearchParams
            this.searchPage = newSearchPage

            // go to 1 page if search changed
            if (
                (isParamsChanged || isQueryChanged) &&
                !isPageChanged &&
                this.searchPage.pageNumber != 1
            ) {
                this.router.navigate([], {
                    relativeTo: this.activatedRoute,
                    queryParams: {
                        ...{ query: this.query },
                        ...this.searchParams,
                        ...new Page(1, this.searchPage.pageSize),
                    },
                    queryParamsHandling: 'merge',
                })
            } else {
                this.search()
            }
        })
    }

    search() {
        this.errorMessages = []
        this.searchResult = null

        this.searchService.searchTopics(this.query, this.searchParams, this.searchPage).subscribe({
            next: (data: SearchResponse) => {
                this.searchResult = data
            },
            error: (err: HttpException) => {
                this.errorMessages = err.errors
            },
        })
    }

    changePage(newPage: number) {
        this.router.navigate([], {
            relativeTo: this.activatedRoute,
            queryParams: {
                ...{ query: this.query },
                ...this.searchParams,
                ...new Page(newPage, this.searchPage.pageSize),
            },
            queryParamsHandling: 'merge',
        })
    }
}
