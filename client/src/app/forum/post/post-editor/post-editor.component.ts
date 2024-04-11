import { ClassicEditor } from '@ckeditor/ckeditor5-editor-classic';
import { PostEditorData } from './../../models/post-editor-data.model';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgForm } from '@angular/forms';
import { NgFormExtension } from 'src/shared/ng-form.extension';
import Editor from 'ckeditor5/build/ckeditor';
import { HttpClient } from '@angular/common/http';
import { FileModel } from '../../models/file.model';
import { UploadService } from '../../services/upload.service';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { take } from 'rxjs';

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
  submitPlaceholder: string = '';
  @Input()
  cancelPlaceholder: string = '';
  @Input()
  inputPlaceholder: string = '';
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
    private toastr: ToastrService,
    trans: TranslateService
    ) {
      trans.get('forms.create')
      .pipe(take(1))
      .subscribe(str => {
        this.submitPlaceholder = str;
      });

      trans.get('labels.cancel')
      .pipe(take(1))
      .subscribe(str => {
        this.cancelPlaceholder = str;
      });

      trans.get('forms.new-message')
      .pipe(take(1))
      .subscribe(str => {
        this.inputPlaceholder = str;
      })
    }

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
