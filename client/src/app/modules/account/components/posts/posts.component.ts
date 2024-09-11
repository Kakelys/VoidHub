import { TranslateService } from '@ngx-translate/core'
import Editor from 'ckeditor5/build/ckeditor'
import { ToastrService } from 'ngx-toastr'
import { ReplaySubject, debounceTime, fromEvent, takeUntil } from 'rxjs'

import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core'
import { ActivatedRoute } from '@angular/router'

import { Offset } from 'src/shared'

import { HttpException, PostInfo } from 'src/app/common/models'
import { ToastrExtension } from 'src/app/common/utils'
import { AuthService, User } from 'src/app/modules/auth'

import { environment } from 'src/environments/environment'

import { AccountService } from '../../services'

@Component({
    selector: 'app-posts',
    templateUrl: './posts.component.html',
    styleUrls: ['./posts.component.css'],
})
export class AccountPostsComponent implements OnInit, OnDestroy {
    editor = Editor as { create: any }
    posts: PostInfo[] = []
    user: User

    loadTime = new Date()
    postLimit = 10
    canLoadMore = true
    loading = false
    id: number

    limitNames = environment.limitNames
    resourceUrl = environment.resourceURL

    @ViewChild('postsContainer', { static: true })
    postsContainer: ElementRef

    private destroy$ = new ReplaySubject<boolean>(1)

    constructor(
        private accService: AccountService,
        private route: ActivatedRoute,
        private toastr: ToastrService,
        private auth: AuthService,
        private trans: TranslateService
    ) {
        this.auth.user$.pipe(takeUntil(this.destroy$)).subscribe((user) => {
            this.user = user
        })
    }

    ngOnInit(): void {
        this.route.parent.params.subscribe((params) => {
            this.handleId(params['id'])
        })

        fromEvent(window, 'scroll')
            .pipe(takeUntil(this.destroy$), debounceTime(300))
            .subscribe((e: any) => {
                const currentScroll =
                    e.target.documentElement.scrollTop + e.target.documentElement.clientHeight
                const blockEnd =
                    this.postsContainer.nativeElement.offsetHeight +
                    this.postsContainer.nativeElement.offsetTop

                if (currentScroll > blockEnd - blockEnd * 0.35) this.loadNextPosts()
            })
    }

    ngOnDestroy(): void {
        this.destroy$.next(true)
        this.destroy$.complete()
    }

    async handleId(id) {
        const newId = id

        if (!Number(newId)) {
            this.trans.get('forms-errors.wrong-account-id').subscribe((str) => {
                this.toastr.error(str)
            })
            return
        }

        if (newId == this.id) return

        this.id = id
        this.loadNextPosts()
    }

    loadNextPosts() {
        if (!this.canLoadMore || this.loading) return
        this.loading = true

        const offset = new Offset(this.posts.length, this.postLimit)

        this.accService.getPosts(this.id, this.loadTime, offset).subscribe({
            next: (data: PostInfo[]) => {
                if (!data || data.length < this.postLimit) {
                    this.canLoadMore = false
                }
                console.log(data)
                this.posts.push(...data)
            },
            error: (err: HttpException) => ToastrExtension.handleErrors(this.toastr, err.errors),
            complete: () => {
                this.loading = false
            },
        })
    }
}
