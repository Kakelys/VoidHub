import { TranslateService } from '@ngx-translate/core'
import { ToastrService } from 'ngx-toastr'
import { ReplaySubject, takeUntil } from 'rxjs'

import { Component, OnDestroy } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'

import { AuthService, User } from 'src/app/modules/auth'

import { HttpException } from 'src/app/common/models'
import { ToastrExtension } from 'src/app/common/utils'

import { environment } from 'src/environments/environment'
import { Page, Roles } from 'src/shared'

import { ForumService, NameService } from '../../../services'
import { ForumResponse, Name, Topic } from '../../../types'

@Component({
    selector: 'app-forum',
    templateUrl: './forum.component.html',
    styleUrls: ['./forum.component.css'],
})
export class ForumComponent implements OnDestroy {
    topics: Topic[] = []
    forumRes: ForumResponse = null

    user: User = null
    private destroy$ = new ReplaySubject<boolean>(1)

    forumId = 0

    showNewTopic = false

    page = new Page(1, 5)

    limitNames = environment.limitNames
    resourceUrl = environment.resourceURL
    roles = Roles
    names: Name[] = null

    imgFile: File | null

    constructor(
        private authService: AuthService,
        private route: ActivatedRoute,
        private router: Router,
        private forumService: ForumService,
        private toastr: ToastrService,
        private trans: TranslateService,
        private nameService: NameService
    ) {
        authService.user$.pipe(takeUntil(this.destroy$)).subscribe((user) => {
            this.user = user
        })

        route.params.subscribe(async (params) => {
            this.handleForumIdChange(params['id'])
        })

        route.queryParams.subscribe((params) => {
            const newPage = new Page(
                +params['page'] ? +params['page'] : this.page.pageNumber,
                +params['pageSize'] ? +params['pageSize'] : this.page.pageSize
            )

            const pageChanged = !this.page.Equals(newPage)
            this.page = newPage
            if (pageChanged) {
                this.loadTopicsPage()
            }
        })
    }

    async handleForumIdChange(newForumId: number) {
        this.forumId = newForumId

        const loadDeleted = this.router.url.indexOf('deleted') != -1
        this.forumService.getForum(this.forumId, loadDeleted).subscribe((forum: ForumResponse) => {
            if (!forum) return

            this.forumRes = forum
            this.loadTopicsPage()
        })
    }

    toggleNewTopic() {
        this.showNewTopic = !this.showNewTopic
    }

    loadTopicsPage() {
        this.topics = []

        const loadDeleted = this.router.url.indexOf('deleted') != -1

        this.forumService
            .getForumTopics(this.forumId, this.page, loadDeleted ? { onlyDeleted: true } : {})
            .subscribe((topics: Topic[]) => {
                this.topics = topics
            })
    }

    handleFileInput(target) {
        this.imgFile = target.files[0]
    }

    onEdit(data) {
        const formData = new FormData()
        Object.keys(data).forEach((k) => {
            if (k && data[k]) {
                formData.append(k, data[k])
            }
        })
        formData.set('img', this.imgFile)
        if (!this.imgFile) formData.delete('img')

        this.forumService.updateForum(this.forumRes.forum.id, formData).subscribe({
            next: (forum: any) => {
                this.forumRes.forum.title = forum.title
                this.forumRes.forum.imagePath = forum.imagePath
                this.trans.get('labels.forum-updated').subscribe((str) => {
                    this.toastr.success(str)
                })
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }

    onEditToggle(state: boolean) {
        if (!state) return

        this.nameService.getSections().subscribe({
            next: (names: Name[]) => {
                this.names = names
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }

    onDelete() {
        this.forumService.deleteForum(this.forumRes.forum.id).subscribe({
            next: () => {
                this.router.navigate(['../../'], { relativeTo: this.route })
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }

    onCreated(topic: Topic) {
        this.router.navigate(['/forum/topic/', topic.id])
    }

    ngOnDestroy(): void {
        this.destroy$.next(true)
        this.destroy$.complete()
    }
}
