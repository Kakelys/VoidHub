import { TranslateModule } from '@ngx-translate/core'
import { ToastrModule, ToastrService } from 'ngx-toastr'

import { CommonModule } from '@angular/common'
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core'
import { takeUntilDestroyed } from '@angular/core/rxjs-interop'
import { FormsModule, NgForm } from '@angular/forms'
import { Router, RouterModule } from '@angular/router'

import { Roles } from 'src/shared'

import { HttpException } from 'src/app/common/models'
import { ToastrExtension } from 'src/app/common/utils'
import { AuthService, User } from 'src/app/modules/auth'
import { Name, NameService, SearchParams } from 'src/app/modules/forum'

import { SearchSort } from '../../types'

@Component({
    selector: 'app-search-bar',
    templateUrl: './search-bar.component.html',
    styleUrls: ['./search-bar.component.css'],
    standalone: true,
    imports: [CommonModule, RouterModule, FormsModule, TranslateModule, ToastrModule],
    providers: [NameService],
})
export class SearchBarComponent implements OnInit {
    isQueryFocused = false

    @Input()
    searchQuery = ''

    @Input()
    searchParams: SearchParams

    @Input()
    enableParams = false

    @Output()
    searchForced = new EventEmitter()

    names: Name[]

    roles = Roles
    user: User
    sortTypes = SearchSort

    constructor(
        private router: Router,
        private nameService: NameService,
        private toastr: ToastrService,
        auth: AuthService
    ) {
        auth.user$.pipe(takeUntilDestroyed()).subscribe((user: User) => {
            this.user = user
        })
    }

    ngOnInit(): void {
        this.nameService.getForums().subscribe({
            next: (names: Name[]) => {
                this.names = names
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }

    onSubmit(form: NgForm) {
        this.router.navigate(['/forum/search'], {
            queryParams: {
                query: form.value.search,
                ...this.searchParams,
                ...form.value,
                v: new Date().getMilliseconds(),
            },
            queryParamsHandling: 'merge',
        })
    }

    onQueryFocus(event: any) {
        this.isQueryFocused = true
    }

    onQueryBlur(event: any) {
        setTimeout((_) => {
            this.isQueryFocused = false
        }, 100)
    }
}
