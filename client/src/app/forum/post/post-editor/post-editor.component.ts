import { ClassicEditor } from '@ckeditor/ckeditor5-editor-classic';
import { PostEditorData } from './../../models/post-editor-data.model';
import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild, ViewChildren, ElementRef } from '@angular/core';
import { NgForm } from '@angular/forms';
import { NgFormExtension } from 'src/shared/ng-form.extension';
import Editor from 'ckeditor5/build/ckeditor';
import { HttpClient } from '@angular/common/http';
import { CKEditorComponent } from '@ckeditor/ckeditor5-angular';
import { FileModel } from '../../models/file.model';
import { UploadService } from '../../services/upload.service';
import { ToastrExtension } from 'src/shared/toastr.extension';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-post-editor',
  templateUrl: './post-editor.component.html',
  styleUrls: ['./post-editor.component.css']
})
export class PostEditorComponent {
  editor = Editor;
  editorInstance: ClassicEditor;

  @Output()
  onCreate = new EventEmitter<PostEditorData>();

  @Output()
  onCancel = new EventEmitter<any>();

  @Input()
  content = '';

  @Input()
  submitPlaceholder: string = 'Create';
  @Input()
  cancelPlaceholder: string = 'Cancel';
  @Input()
  inputPlaceholder: string = 'Topic message';
  @Input()
  cancelClassesReplacement: string | null;

  @Input()
  ancestorId: number | null = null;
  @Input()
  topicId: number;
  @Input()
  postId: number | null = null;

  uploadedFiles: FileModel[] = [];

  constructor(
    private http: HttpClient,
    private uploadService: UploadService,
    private toastr: ToastrService
    ) {}

  onEditorReady(instace: ClassicEditor): void {
    this.editorInstance = instace;
  }

  addImage(url: string): void {
    this.editorInstance.execute( 'insertImage', { source: url } );
  }

  onSubmit(form: NgForm): void {
    if(form.invalid) {
      NgFormExtension.markAllAsTouched(form);
      return;
    }

    const data: PostEditorData = form.value;
    data.fileIds = this.uploadedFiles.map(f => f.id);

    this.onCreate.emit(data);
  }

  onCancelClick(): void {
    this.content = '';
    this.onCancel.emit();
  }

  onFilesUpdated(files: FileModel[]): void {
    this.uploadedFiles = files;
  }
}
