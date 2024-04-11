import { Component, EventEmitter, Input, Output } from '@angular/core';
import { User } from 'src/shared/models/user.model';
import { PostService } from '../../services/post.service';
import { Roles } from 'src/shared/roles.enum';
import Editor from 'ckeditor5/build/ckeditor';
import { environment } from 'src/environments/environment';
import { PostInfo } from 'src/shared/models/post-info.model';
import { HttpException } from 'src/shared/models/http-exception.model';
import { ToastrService } from 'ngx-toastr';
import { ToastrExtension } from 'src/shared/toastr.extension';

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

  constructor(private postService: PostService,
    private toastr: ToastrService) {
  }

  onPostEdit(data) {
    this.postService.updatePost(this.post.post.id, data).subscribe({
      next: (post: any) => {
        this.editMode = false;
        this.post.post.content = post.content;
      },
      error: (err: HttpException) => {
        ToastrExtension.handleErrors(this.toastr, err.errors);
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

  toggleCommentsMode() {
    this.commentsMode = !this.commentsMode;
  }

  updateCommentsCounter(count) {
    this.post.post.commentsCount = count;
  }

  changeLikeState() {
    if(!this.user)
      return;

    if(!this.post.post.isLiked) {
      this.post.post.isLiked = true;
      this.postService.like(this.post.post.id)
      .subscribe({
        next: _ => {this.post.post.likesCount++},
        error: _ => {this.post.post.isLiked = false;}
      })
    }
    else {
      this.post.post.isLiked = false;
      this.postService.unlike(this.post.post.id)
      .subscribe({
        next: _ => {this.post.post.likesCount--},
        error: _ => {this.post.post.isLiked = true;}
      })
    }
  }
}
