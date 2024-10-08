import { ToastrService } from 'ngx-toastr'
import { ReplaySubject, takeUntil } from 'rxjs'

import { Component, OnDestroy } from '@angular/core'
import { ActivatedRoute, Router } from '@angular/router'

import { Page, Roles } from 'src/shared'

import { HttpException } from 'src/app/common/models'
import { ToastrExtension } from 'src/app/common/utils'
import { AuthService, User } from 'src/app/modules/auth'

import { environment as env } from 'src/environments/environment'

import { NameService, PostService, TopicService } from '../../../services'
import { Name, Topic, TopicDetail } from '../../../types'

@Component({
    selector: 'app-topic',
    templateUrl: './topic.component.html',
    styleUrls: ['./topic.component.css'],
})
export class TopicComponent implements OnDestroy {
    topic: TopicDetail = null
    posts: any[] = []
    names: Name[]

    user: User = null
    roles = Roles

    postsOnPage = 5
    currentPage = -1
    topicId

    newPostContent = ''

    limitNames = env.limitNames

    private destroy$ = new ReplaySubject<boolean>()
    private firstTopicLoad = true

    constructor(
        authService: AuthService,
        private topicService: TopicService,
        private postService: PostService,
        private router: Router,
        private route: ActivatedRoute,
        private toastr: ToastrService,
        private nameService: NameService
    ) {
        authService.user$.pipe(takeUntil(this.destroy$)).subscribe((user) => {
            this.user = user
        })

        this.route.params.subscribe(async (params) => {
            await this.handleNewPage(params['page'])
            this.handleNewTopicId(params['id'])
        })
    }

    async handleNewTopicId(newTopicId: number) {
        this.firstTopicLoad = false
        if (newTopicId == this.topicId) return

        const offset = new Page(this.currentPage, this.postsOnPage).getOffset()
        this.topicId = newTopicId

        if (+this.topicId) {
            this.topicService.getTopic(this.topicId, offset).subscribe({
                next: (topic: any) => {
                    this.topic = topic
                    this.posts = topic.posts
                },
            })
        }
    }

    async handleNewPage(newPage) {
        if (newPage == this.currentPage) return

        this.currentPage = newPage

        // skip loading if topic is not loaded yet
        if (!this.firstTopicLoad) this.loadNewPostsPage()
    }

    changePage(page: number) {
        this.router.navigate(['../', page], { relativeTo: this.route })
    }

    loadNewPostsPage() {
        const offset = new Page(this.currentPage, this.postsOnPage).getOffset()

        this.postService.getComments(this.topic?.post.id, offset, null).subscribe({
            next: (posts: any[]) => {
                this.posts = []
                this.posts.push(...posts)
            },
        })
    }

    //post methods
    onPostDelete() {
        this.topic.topic.postsCount--
        this.loadNewPostsPage()
    }

    onPostCreate(data) {
        this.postService.createPost(data).subscribe({
            next: (data) => {
                this.topic.topic.postsCount++
                //because one-way binding
                this.newPostContent = new Date().toString()
                setTimeout(() => {
                    this.newPostContent = ''
                })

                let page = this.topic.topic.postsCount / this.postsOnPage
                page = page % 1 > 0 ? Math.floor(page + 1) : page
                if (page == this.currentPage) this.loadNewPostsPage()
                else this.changePage(page)
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }

    //topic methods
    onTopicEdit(data) {
        this.topicService.updateTopic(this.topic.topic.id, data).subscribe({
            next: (topicResponse: Topic) => {
                //also save old values
                this.topic.topic = {
                    ...this.topic,
                    ...topicResponse,
                }
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }

    onTopicEditChanged(state: boolean) {
        if (!state) return

        this.nameService.getForums().subscribe({
            next: (names: Name[]) => {
                this.names = names
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }

    onTopicDelete() {
        this.topicService.deleteTopic(this.topic.topic.id).subscribe({
            next: () => {
                this.router.navigate(['/', 'forum', this.topic.topic.forumId])
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }

    onConfirmRecover() {
        this.topicService.recoverTopic(this.topic.topic.id).subscribe({
            next: (topic: Topic) => {
                this.topic.topic.deletedAt = null
                this.loadNewPostsPage()
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }

    ngOnDestroy(): void {
        this.destroy$.next(true)
        this.destroy$.complete()
    }
}
