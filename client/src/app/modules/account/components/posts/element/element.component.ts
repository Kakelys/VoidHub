import { ToastrService } from 'ngx-toastr'

import { Component } from '@angular/core'

import { PostElementComponent, PostService } from 'src/app/modules/forum'

@Component({
    selector: 'app-account-post-element',
    templateUrl: './element.component.html',
    styleUrls: ['./element.component.css'],
})
export class AccountPostElementComponent extends PostElementComponent {
    constructor(postService: PostService, toastr: ToastrService) {
        super(postService, toastr)
    }
}
