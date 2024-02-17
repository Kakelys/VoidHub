import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { PostService } from '../../services/post.service';
import { UploadService } from '../../services/upload.service';
import { FileModel } from '../../models/file.model';
import { ToastrService } from 'ngx-toastr';
import { environment as env } from 'src/environments/environment';
import { ToastrExtension } from 'src/shared/toastr.extension';
import { ReplaySubject } from 'rxjs';

@Component({
  selector: 'app-upload-images',
  templateUrl: './upload-images.component.html',
  styleUrls: ['./upload-images.component.css']
})
export class UploadImagesComponent implements OnInit, OnDestroy {

  resourceUrl: string = env.resourceURL;

  @Input()
  postId: number | null = null;

  uploadedFiles: FileModel[] = [];

  private destroy$ = new ReplaySubject<boolean>();

  @Output()
  onFilesUpdates = new EventEmitter<FileModel[]>();
  @Output()
  onPaste = new EventEmitter<string>();

  constructor(
    private postService: PostService,
    private uploadService: UploadService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.postService.postCreated$.subscribe({
      next: _ => {
        this.uploadedFiles = [];
        this.onFilesUpdates.emit(this.uploadedFiles);
      }
    })

    if(!this.postId)
      return;

    this.postService.getImages(this.postId).subscribe({
      next: (files: FileModel[]) => {
        this.uploadedFiles = files;
        this.onFilesUpdates.emit(files);
      },
      error: errs => {
        ToastrExtension.handleErrors(this.toastr, errs);
      }
    })
  }

  ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.complete();

    if(this.postId)
      return;

    let ids = this.uploadedFiles.map(f => f.id);
    if(ids.length > 0)
    {
      this.uploadService.deleteMany(ids).subscribe({
        error: errs => ToastrExtension.handleErrors(this.toastr, errs)
      })
    }
  }

  handleFileInput(target: any) {
    let files = target.files;
    if(files.lengh == 0)
      return;

    if(files[0] > env.maxAvatarSize) {
      this.toastr.error(`File size must be less than ${env.maxAvatarSize / 1024} KB`);
      return;
    }

    let formData = new FormData();
    formData.append('file', files[0]);
    if(this.postId)
      formData.append('postId', this.postId + '');

    this.uploadService.upload(formData).subscribe({
      next: (file: FileModel) => {
        console.log(file);
        this.uploadedFiles.push(file);
        this.onFilesUpdates.emit(this.uploadedFiles);
      },
      error: errs => {
        ToastrExtension.handleErrors(this.toastr, errs)
      }
    })
  }

  onDelete(id: number) {
    this.uploadService.delete(id).subscribe({
      next: _ => {
        this.uploadedFiles = this.uploadedFiles.filter(f => f.id != id);
        this.onFilesUpdates.emit(this.uploadedFiles);
      },
      error: errs => ToastrExtension.handleErrors(this.toastr, errs)
    })
  }

  emitPaste(url: string) {
    this.onPaste.emit(url);
  }
}
