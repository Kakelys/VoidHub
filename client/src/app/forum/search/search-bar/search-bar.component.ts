import { TranslateModule } from '@ngx-translate/core'
import { ToastrModule, ToastrService } from 'ngx-toastr'

import { CommonModule } from '@angular/common'
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core'
import { takeUntilDestroyed } from '@angular/core/rxjs-interop'
import { FormsModule, NgForm } from '@angular/forms'
import { Router, RouterModule } from '@angular/router'

import { HttpException } from 'src/shared/models/http-exception.model'
import { Roles } from 'src/shared/roles.enum'
import { ToastrExtension } from 'src/shared/toastr.extension'

import { AuthService } from 'src/app/auth/auth.service'

import { User } from '../../../../shared/models/user.model'
import { Name } from '../../models/names.model'
import { SearchParams } from '../../models/search-params.model'
import { NameService } from './../../services/name.service'
import { SearchSort } from './../search-sort.enum'

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
