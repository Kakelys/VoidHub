import { ToastrService } from 'ngx-toastr'

import { Component, Input } from '@angular/core'

import { PostService } from '../../../services'
import { PostElementComponent } from '../../post/post-element/post-element.component'

@Component({
    selector: 'app-recent-element',
    templateUrl: './recent-element.component.html',
    styleUrls: ['./recent-element.component.css'],
})
export class RecentElementComponent extends PostElementComponent {
    @Input()
    symbolLimit = 200

    @Input()
    tagLimit = 11

    constructor(postService: PostService, toastr: ToastrService) {
        super(postService, toastr)
    }
}
