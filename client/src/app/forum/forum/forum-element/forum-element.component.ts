import { Component, Input } from '@angular/core'

import { environment as env } from 'src/environments/environment'

import { ForumResponse } from '../../models/forum-response.model'

@Component({
    selector: 'app-forum-element',
    templateUrl: './forum-element.component.html',
    styleUrls: ['./forum-element.component.css'],
})
export class ForumElementComponent {
    @Input()
    forumRes: ForumResponse

    resourceUrl = env.resourceURL
}
