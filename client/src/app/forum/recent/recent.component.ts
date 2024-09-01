import Editor from 'ckeditor5/build/ckeditor'
import { ReplaySubject, debounceTime, fromEvent, takeUntil } from 'rxjs'

import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core'

import { User } from 'src/shared/models/user.model'
import { Offset } from 'src/shared/offset.model'

import { AuthService } from 'src/app/auth/auth.service'

import { environment } from 'src/environments/environment'

import { TopicInfo } from '../models/topic-info.model'
import { TopicService } from '../services/topic.service'

@Component({
    selector: 'app-recent',
    templateUrl: './recent.component.html',
    styleUrls: ['./recent.component.css'],
})
export class RecentComponent implements OnInit, OnDestroy {
    editor = Editor as { create: any }
    resourceUrl = environment.resourceURL
    limitNames = environment.limitNames

    private firstLoadTime: Date = new Date()
    topicLimit = 10

    topics: TopicInfo[] = []
    user: User

    @ViewChild('topicsContainer', { static: true })
    topicsContainer: ElementRef

    canLoadMore = true
    loading = false
    threshold = 400

    private destroy$ = new ReplaySubject<boolean>(1)

    constructor(
        private topicService: TopicService,
        private auth: AuthService
    ) {
        this.auth.user$.pipe(takeUntil(this.destroy$)).subscribe((user) => {
            this.user = user
        })
    }

    ngOnInit(): void {
        this.loadNext()

        fromEvent(window, 'scroll')
            .pipe(takeUntil(this.destroy$), debounceTime(300))
            .subscribe((e: any) => {
                const currentScroll =
                    e.target.documentElement.scrollTop + e.target.documentElement.clientHeight
                const blockEnd =
                    this.topicsContainer.nativeElement.offsetHeight +
                    this.topicsContainer.nativeElement.offsetTop

                if (currentScroll > blockEnd - blockEnd * 0.35) this.loadNext()
            })
    }

    loadNext() {
        if (!this.canLoadMore || this.loading) return
        this.loading = true

        // TODO: remove old after some cap
        const offset = new Offset(this.topics.length, this.topicLimit)
        this.topicService.getTopics(offset, this.firstLoadTime).subscribe({
            next: (topics: TopicInfo[]) => {
                if (!topics || topics.length < this.topicLimit) this.canLoadMore = false
                this.topics.push(...topics)
            },
            complete: () => {
                this.loading = false
            },
        })
    }

    ngOnDestroy(): void {
        this.destroy$.next(true)
        this.destroy$.complete()
    }
}
