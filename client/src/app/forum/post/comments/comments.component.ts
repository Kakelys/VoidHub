import { Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { User } from 'src/shared/models/user.model';
import { PostService } from '../../services/post.service';
import { Offset } from 'src/shared/offset.model';
import { environment as env } from 'src/environments/environment';
import { ReplaySubject, debounceTime, fromEvent, takeUntil } from 'rxjs';

@Component({
  selector: 'app-comments',
  templateUrl: './comments.component.html',
  styleUrls: ['./comments.component.css']
})
export class CommentsComponent implements OnInit, OnDestroy {

  @Input()
  user: User;

  @Input()
  topicId;

  @Input()
  postId;

  @Input()
  commentsCount = 0;

  @Input()
  depth = 1;
  depthLimit = env.commenthDepthLimit;

  @Input()
  isTopicClosed: boolean = false;

  @Output()
  onCommentsCounterUpdated = new EventEmitter<number>();

  @Output()
  onClose = new EventEmitter();

  posts:any = [];
  postOnPage = 5;
  loadTime: Date = new Date();
  canLoadMore = true;
  loading = false;

  fullLoad$ = new ReplaySubject<boolean>(1);
  @ViewChild('commentsContainer', {static: true})
  commentsContainer: ElementRef;

  editorContent = '';

  constructor(private postService: PostService) {}

  ngOnInit(): void {
    this.loadNextPosts();

    fromEvent(window, 'scroll')
    .pipe(takeUntil(this.fullLoad$), debounceTime(300))
    .subscribe((e:any) => {
      const currentScroll = e.target.documentElement.scrollTop + e.target.documentElement.clientHeight;
      const blockEnd = this.commentsContainer.nativeElement.offsetHeight + this.commentsContainer.nativeElement.offsetTop;

      if(currentScroll > blockEnd - blockEnd * 0.35)
      this.loadNextPosts();
    })
  }

  ngOnDestroy(): void {
    this.fullLoad$.next(true);
    this.fullLoad$.complete();
  }

  loadNextPosts() {
    if(!this.canLoadMore || this.loading)
      return;

    this.loading = true;

    let offset = new Offset(this.posts.length, this.postOnPage);

    this.postService.getComments(this.postId, offset, null).subscribe({
      next: (posts: any[]) => {
        if(posts.length == 0 || posts.length < this.postOnPage) {
          this.canLoadMore = false;

          this.fullLoad$.next(true);
          this.fullLoad$.complete();
        }
        this.posts.push(...posts);
      },
      complete: () => {
        this.loading = false;
      }
    });
  }

  onPostCreate(data) {
    this.postService.createPost(data).subscribe({
      next: _ => {
        this.onCommentsCounterUpdated.emit(this.commentsCount + 1);
        this.commentsCount += 1;
        this.canLoadMore = true;

        this.loadNextPosts();

        //clear the editor
        this.editorContent = new Date().getMilliseconds() + '';
        setTimeout(() => this.editorContent = '', 0)
      }
    });
  }

  onPostDelete(postId: any) {
    this.posts.splice(this.posts.findIndex(p => p.id == postId), 1);
    this.onCommentsCounterUpdated.emit(this.commentsCount - 1);
    this.commentsCount -= 1;
  }

  onCloseClicked() {
    this.onClose.emit();
  }
}
