import Editor from 'ckeditor5/build/ckeditor'
import { ToastrService } from 'ngx-toastr'

import { Component, EventEmitter, Input, Output } from '@angular/core'

import { Roles } from 'src/shared/roles.enum'

import { HttpException, Post, PostInfo } from 'src/app/common/models'
import { ToastrExtension } from 'src/app/common/utils'
import { User } from 'src/app/modules/auth'

import { environment } from 'src/environments/environment'

import { PostService } from '../../../services'

@Component({
    selector: 'app-post',
    templateUrl: './post-element.component.html',
    styleUrls: ['./post-element.component.css'],
})
export class PostElementComponent {
    editor = Editor as { create: any }
    resourceUrl = environment.resourceURL

    @Input()
    post: PostInfo

    @Input()
    user: User

    @Input()
    enableDeliting = true

    @Input()
    enableComments = true

    @Input()
    isTopicClosed = false

    @Input()
    depth = 1

    roles = Roles

    @Output()
    deleted = new EventEmitter<number>()

    editMode = false
    commentsMode = false

    constructor(
        private postService: PostService,
        private toastr: ToastrService
    ) {}

    onPostEdit(data) {
        this.postService.updatePost(this.post.post.id, data).subscribe({
            next: (post: Post) => {
                this.editMode = false
                this.post.post.content = post.content
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }

    setEditMode(value: boolean) {
        this.editMode = value
    }

    onAdminDelete() {
        this.postService.deletePost(this.post.post.id).subscribe({
            next: (_) => this.handleDelete(),
        })
    }

    handleDelete() {
        this.deleted.emit(this.post.post.id)
    }

    toggleCommentsMode() {
        this.commentsMode = !this.commentsMode
    }

    updateCommentsCounter(count) {
        this.post.post.commentsCount = count
    }

    changeLikeState() {
        if (!this.user) return

        if (!this.post.post.isLiked) {
            this.post.post.isLiked = true
            this.postService.like(this.post.post.id).subscribe({
                next: (_) => {
                    this.post.post.likesCount++
                },
                error: (_) => {
                    this.post.post.isLiked = false
                },
            })
        } else {
            this.post.post.isLiked = false
            this.postService.unlike(this.post.post.id).subscribe({
                next: (_) => {
                    this.post.post.likesCount--
                },
                error: (_) => {
                    this.post.post.isLiked = true
                },
            })
        }
    }
}
