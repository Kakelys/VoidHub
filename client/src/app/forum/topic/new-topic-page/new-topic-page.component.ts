import { Component } from '@angular/core'
import { takeUntilDestroyed } from '@angular/core/rxjs-interop'
import { Router } from '@angular/router'

import { Topic } from '../../models/topic.model'
import { TopicService } from '../../services/topic.service'

@Component({
    selector: 'app-new-topic-page',
    templateUrl: './new-topic-page.component.html',
    styleUrls: ['./new-topic-page.component.css'],
})
export class NewTopicPageComponent {
    constructor(
        private router: Router,
        private topicService: TopicService
    ) {
        this.topicService.topicCreated$.pipe(takeUntilDestroyed()).subscribe((topic: Topic) => {
            this.router.navigate(['/forum/topic/', topic.id])
        })
    }
}
