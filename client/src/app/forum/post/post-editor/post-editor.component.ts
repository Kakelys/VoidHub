import { ClassicEditor } from '@ckeditor/ckeditor5-editor-classic'
import { TranslateService } from '@ngx-translate/core'
import Editor from 'ckeditor5/build/ckeditor'
import { ToastrService } from 'ngx-toastr'
import { take } from 'rxjs'

import { HttpClient } from '@angular/common/http'
import { Component, EventEmitter, Input, Output } from '@angular/core'
import { NgForm } from '@angular/forms'

import { NgFormExtension } from 'src/shared/ng-form.extension'

import { FileModel } from '../../models/file.model'
import { UploadService } from '../../services/upload.service'
import { PostEditorData } from './../../models/post-editor-data.model'

@Component({
    selector: 'app-post-editor',
    templateUrl: './post-editor.component.html',
    styleUrls: ['./post-editor.component.css'],
})
export class PostEditorComponent {
    editor = Editor
    editorInstance: ClassicEditor

    @Output()
    created = new EventEmitter<PostEditorData>()

    @Output()
    canceled = new EventEmitter<void>()

    @Input()
    content = ''

    @Input()
    submitPlaceholder = ''

    @Input()
    cancelPlaceholder = ''

    @Input()
    inputPlaceholder = ''

    @Input()
    cancelClassesReplacement: string | null

    @Input()
    ancestorId: number | null = null
    @Input()
    topicId: number
    @Input()
    postId: number | null = null

    uploadedFiles: FileModel[] = []

    constructor(
        private http: HttpClient,
        private uploadService: UploadService,
        private toastr: ToastrService,
        trans: TranslateService
    ) {
        trans
            .get('forms.create')
            .pipe(take(1))
            .subscribe((str) => {
                this.submitPlaceholder = str
            })

        trans
            .get('labels.cancel')
            .pipe(take(1))
            .subscribe((str) => {
                this.cancelPlaceholder = str
            })

        trans
            .get('forms.new-message')
            .pipe(take(1))
            .subscribe((str) => {
                this.inputPlaceholder = str
            })
    }

    onEditorReady(instance: ClassicEditor): void {
        this.editorInstance = instance
    }

    addImage(url: string): void {
        const ext = url.split('.').pop()

        if (ext == 'mp4') this.editorInstance.execute('mediaEmbed', url)
        else this.editorInstance.execute('insertImage', { source: url })
    }

    onSubmit(form: NgForm): void {
        if (form.invalid) {
            NgFormExtension.markAllAsTouched(form)
            return
        }

        const data: PostEditorData = form.value
        data.fileIds = this.uploadedFiles.map((f) => f.id)

        this.created.emit(data)
    }

    onCancelClick(): void {
        this.content = ''
        this.canceled.emit()
    }

    onFilesUpdated(files: FileModel[]): void {
        this.uploadedFiles = files
    }
}
