import { Component, Input } from '@angular/core'

import { ForumResponse } from '../../models/forum-response.model'

@Component({
    selector: 'app-forum-list',
    templateUrl: './forum-list.component.html',
    styleUrls: ['./forum-list.component.css'],
})
export class ForumListComponent {
    @Input()
    data: ForumResponse[] = []
}
