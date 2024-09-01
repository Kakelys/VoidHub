import Editor from 'ckeditor5/build/ckeditor'
import { ToastrService } from 'ngx-toastr'

import { Component, EventEmitter, OnInit, Output } from '@angular/core'
import { NgForm } from '@angular/forms'
import { ActivatedRoute, Router } from '@angular/router'

import { NgFormExtension } from 'src/shared'

import { HttpException } from 'src/app/common/models'
import { ToastrExtension } from 'src/app/common/utils'

import { NameService, TopicService } from '../../../services'
import { Name, PostEditorData, Topic } from '../../../types'

@Component({
    selector: 'app-new-topic',
    templateUrl: './new-topic.component.html',
    styleUrls: ['./new-topic.component.css'],
})
export class NewTopicComponent implements OnInit {
    editor = Editor as {
        create: any
    }

    forumId
    errorMessages: string[] = []
    content = ''

    @Output()
    closed = new EventEmitter()

    @Output()
    created = new EventEmitter<Topic>()

    names: Name[] = null

    constructor(
        route: ActivatedRoute,
        private router: Router,
        private topicService: TopicService,
        private nameService: NameService,
        private toastr: ToastrService
    ) {
        route.params.subscribe((params) => {
            this.forumId = params['id']
        })
    }

    ngOnInit(): void {
        this.nameService.getForums().subscribe({
            next: (names: Name[]) => {
                this.names = names
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }

    onSubmit(form: NgForm, postData: PostEditorData) {
        this.errorMessages = []

        if (form.invalid) {
            NgFormExtension.markAllAsTouched(form)
            return
        }

        form.value.content = postData.content
        form.value.fileIds = postData.fileIds

        this.topicService.createTopic(form.value).subscribe({
            next: (topic: Topic) => {
                this.created.emit(topic)
            },
            error: (err: HttpException) => {
                this.errorMessages = err.errors
            },
        })
    }

    onCancel() {
        this.closed.emit()
    }
}
