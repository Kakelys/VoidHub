import { Component, EventEmitter, Input, Output } from '@angular/core';
import { User } from 'src/shared/models/user.model';
import { PostService } from '../../services/post.service';
import { Roles } from 'src/shared/roles.enum';
import Editor from 'ckeditor5/build/ckeditor';
import { environment } from 'src/environments/environment';
import { Post } from 'src/shared/models/post-model';
import { PostInfo } from 'src/shared/models/post-info.model';

@Component({
  selector: 'app-post',
  templateUrl: './post-element.component.html',
  styleUrls: ['./post-element.component.css']
})
export class PostElementComponent {
  editor = Editor as {create: any}
  resourceUrl = environment.resourceURL;

  @Input()
  post: PostInfo;

  @Input()
  user: User;

  @Input()
  enableDeliting: boolean = true;

  @Input()
  enableComments: boolean = true;

  @Input()
  isTopicClosed: boolean = false;

  @Input()
  depth = 1;

  roles = Roles;

  @Output()
  onDelete = new EventEmitter<number>();

  editMode = false;
  commentsMode = false;

  constructor(private postService: PostService) {

  }

  onPostEdit(data) {
    this.postService.updatePost(this.post.post.id, data).subscribe({
      next: (post: any) => {
        this.editMode = false;
        this.post.post.content = post.content;
      }
    })
  }

  setEditMode(value: boolean) {
    this.editMode = value;
  }

  onAdminDelete() {
    this.postService.deletePost(this.post.post.id).subscribe({
      next: _ => this.handleDelete(),
    });
  }

  handleDelete() {
    this.onDelete.emit(this.post.post.id);
  }

  // comments
  toggleCommentsMode() {
    this.commentsMode = !this.commentsMode;
  }

  updateCommentsCounter(count) {
    this.post.post.commentsCount = count;
  }
}
