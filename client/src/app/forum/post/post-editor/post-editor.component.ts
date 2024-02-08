import { PostEditorData } from './../../models/post-editor-data.model';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgForm } from '@angular/forms';
import { NgFormExtension } from 'src/shared/ng-form.extension';
import Editor from 'ckeditor5/build/ckeditor';

@Component({
  selector: 'app-post-editor',
  templateUrl: './post-editor.component.html',
  styleUrls: ['./post-editor.component.css']
})
export class NewPostComponent {
  editor = Editor as {
    create: any;
  };

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
  postId: number;

  onSubmit(form: NgForm) {
    if(form.invalid) {
      NgFormExtension.markAllAsTouched(form);
      return;
    }

    const data: PostEditorData = form.value;
    this.onCreate.emit(data);
  }

  onCancelClick() {
    this.content = '';
    this.onCancel.emit();
  }
}
