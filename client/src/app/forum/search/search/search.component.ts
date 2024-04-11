import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SearchParams } from '../../models/search-params.model';
import { Page } from 'src/shared/page.model';
import { SearchService } from '../../services/search.service';
import { HttpException } from 'src/shared/models/http-exception.model';
import { SearchResponse } from '../../models/search-response.model';
import { AuthService } from 'src/app/auth/auth.service';
import { User } from 'src/shared/models/user.model';
import { StringExtension } from 'src/shared/string.extension';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent {

  searchResult: SearchResponse | null;

  searchParams : SearchParams;
  searchPage: Page = new Page(1);
  query = '';

  currentPage = 1;
  errorMessages = [];

  user: User;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private searchService: SearchService,
    auth: AuthService
    ) {

    auth.user$
    .pipe(takeUntilDestroyed())
    .subscribe(user => {
      this.user = user;
    })

    this.activatedRoute
    .queryParams.subscribe(params => {
      // mb just use ...params instead
      let newSearchParams: SearchParams = {
        sort:'',
        withPostContent:false,
        onlyDeleted: false
      };

      newSearchParams.sort = params["sort"] ?? '';
      newSearchParams.withPostContent = StringExtension.ConvertToBoolean(params["withPostContent"]) ?? false;
      newSearchParams.onlyDeleted = StringExtension.ConvertToBoolean(params["onlyDeleted"]) ?? false;

      let newSearchPage = new Page(
        +params["pageNumber"] ? +params["pageNumber"] : this.searchPage.pageNumber,
        +params["pageSize"] ? +params["pageSize"] : this.searchPage.pageSize,
      );

      let newQuery = params["query"];

      let isParamsChanged = this.searchParams+'' != newSearchParams+'';
      let isQueryChanged = this.query != newQuery;
      let isPageChanged = !this.searchPage.Equals(newSearchPage);

      this.query = newQuery ?? "";
      this.searchParams = newSearchParams;
      this.searchPage = newSearchPage;

      // go to 1 page if search changed
      if((isParamsChanged || isQueryChanged) && !isPageChanged && this.searchPage.pageNumber != 1) {
        this.router.navigate([], {
          relativeTo: this.activatedRoute,
          queryParams: {
            ...{query: this.query},
            ...this.searchParams,
            ...new Page(1, this.searchPage.pageSize)
          },
          queryParamsHandling: 'merge'
        });
      }
      else {
        this.search();
      }
    });
  }

  search() {
    this.errorMessages = [];
    this.searchResult = null;

    this.searchService.searchTopics(this.query, this.searchParams, this.searchPage)
    .subscribe({
      next: (data: SearchResponse) => {
        this.searchResult = data;
      },
      error: (err:HttpException) => {
        this.errorMessages = err.errors;
      }
    })
  }

  changePage(newPage: number) {
    this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: {
        ...{query: this.query},
        ...this.searchParams,
        ...new Page(newPage, this.searchPage.pageSize)
      },
      queryParamsHandling: 'merge'
    });
  }
}
