import { TranslateService } from '@ngx-translate/core'
import { ToastrService } from 'ngx-toastr'
import { ReplaySubject, takeUntil } from 'rxjs'

import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core'

import { HttpException } from 'src/app/common/models'
import { ToastrExtension } from 'src/app/common/utils'

import { environment as env } from 'src/environments/environment'

import { PostService, TopicService, UploadService } from '../../../services'
import { FileModel } from '../../../types'

@Component({
    selector: 'app-upload-images',
    templateUrl: './upload-images.component.html',
    styleUrls: ['./upload-images.component.css'],
})
export class UploadImagesComponent implements OnInit, OnDestroy {
    resourceUrl: string = env.resourceURL

    @Input()
    postId: number | null = null

    uploadedFiles: FileModel[] = []

    private destroy$ = new ReplaySubject<boolean>()

    @Output()
    filesUpdated = new EventEmitter<FileModel[]>()
    @Output()
    inserted = new EventEmitter<string>()

    constructor(
        private postService: PostService,
        private topicService: TopicService,
        private uploadService: UploadService,
        private toastr: ToastrService,
        private trans: TranslateService
    ) {}

    ngOnInit(): void {
        this.postService.postCreated$.pipe(takeUntil(this.destroy$)).subscribe({
            next: (_) => {
                this.uploadedFiles = []
                this.filesUpdated.emit(this.uploadedFiles)
            },
        })

        this.topicService.topicCreated$.pipe(takeUntil(this.destroy$)).subscribe({
            next: (_) => {
                this.uploadedFiles = []
                this.filesUpdated.emit(this.uploadedFiles)
            },
        })

        if (!this.postId) return

        this.postService.getImages(this.postId).subscribe({
            next: (files: FileModel[]) => {
                this.uploadedFiles = files
                this.filesUpdated.emit(files)
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }

    ngOnDestroy(): void {
        this.destroy$.next(true)
        this.destroy$.complete()

        if (this.postId) return

        const ids = this.uploadedFiles.map((f) => f.id)
        if (ids.length > 0) {
            this.uploadService.deleteMany(ids).subscribe({
                error: (err: HttpException) =>
                    ToastrExtension.handleErrors(this.toastr, err.errors),
            })
        }
    }

    handleFileInput(target: any) {
        const files = target.files
        if (files.lengh == 0) return

        if (files[0] > env.maxAvatarSize) {
            this.trans.get('forms-errors.file-size').subscribe((str) => {
                this.toastr.error(`${str} ${env.maxAvatarSize / 1024} KB`)
            })
            return
        }

        const formData = new FormData()
        formData.append('file', files[0])
        if (this.postId) formData.append('postId', this.postId + '')

        this.uploadService.upload(formData).subscribe({
            next: (file: FileModel) => {
                this.uploadedFiles.push(file)
                this.filesUpdated.emit(this.uploadedFiles)
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }

    copyToClipBoard(url: string) {
        // not working over http
        navigator.clipboard.writeText(url).then((_) => {
            this.trans.get('labels.copied').subscribe((str) => {
                this.toastr.success(str + '!')
            })
        })
    }

    onDelete(id: number) {
        this.uploadService.delete(id).subscribe({
            next: (_) => {
                this.uploadedFiles = this.uploadedFiles.filter((f) => f.id != id)
                this.filesUpdated.emit(this.uploadedFiles)
            },
            error: (err: HttpException) => ToastrExtension.handleErrors(this.toastr, err.errors),
        })
    }

    emitPaste(url: string) {
        this.inserted.emit(url)
    }
}
