import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AccountService } from '../account.service';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ReplaySubject, debounceTime, fromEvent, takeUntil } from 'rxjs';
import { Offset } from 'src/shared/offset.model';
import { PostInfo } from 'src/shared/models/post-info.model';
import { ToastrExtension } from 'src/shared/toastr.extension';
import { HttpException } from 'src/shared/models/http-exception.model';
import { environment } from 'src/environments/environment';
import Editor from 'ckeditor5/build/ckeditor';
import { User } from 'src/shared/models/user.model';
import { AuthService } from 'src/app/auth/auth.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-posts',
  templateUrl: './posts.component.html',
  styleUrls: ['./posts.component.css']
})
export class AccountPostsComponent implements OnInit, OnDestroy {

  editor = Editor as {create: any}
  posts: PostInfo[] = []
  user: User;

  loadTime = new Date();
  postLimit = 10;
  canLoadMore = true;
  loading = false;
  id: number;

  limitNames = environment.limitNames;
  resourceUrl = environment.resourceURL;

  @ViewChild('postsContainer', {static: true})
  postsContainer: ElementRef;

  private destroy$ = new ReplaySubject<boolean>(1);

  constructor(
  private accService: AccountService,
  private route: ActivatedRoute,
  private toastr: ToastrService,
  private auth: AuthService,
  private trans: TranslateService) {
    this.auth.user$
    .pipe(takeUntil(this.destroy$))
    .subscribe(user => {
      this.user = user;
    })
  }

  ngOnInit(): void {
    this.route.parent.params.subscribe(params => {
      this.handleId(params['id']);
    })

    fromEvent(window, 'scroll')
    .pipe(takeUntil(this.destroy$), debounceTime(300))
    .subscribe((e:any) => {
      const currentScroll = e.target.documentElement.scrollTop + e.target.documentElement.clientHeight;
      const blockEnd = this.postsContainer.nativeElement.offsetHeight + this.postsContainer.nativeElement.offsetTop;

      if(currentScroll > blockEnd - blockEnd * 0.35)
        this.loadNextPosts();
    })
  }

  ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.complete();
  }

  async handleId(id) {
    let newId = id;

    if(!Number(newId)) {
      this.trans.get('forms-errors.wrong-account-id')
      .subscribe(str => {
        this.toastr.error(str);
      })
      return;
    }

    if(newId == this.id)
      return;

    this.id = id;
    this.loadNextPosts();
  }

  loadNextPosts() {
    if(!this.canLoadMore || this.loading)
      return;
    this.loading = true;

    const offset = new Offset(this.posts.length, this.postLimit);

    this.accService.getPosts(this.id, this.loadTime, offset)
    .subscribe({
      next: (data: PostInfo[]) => {
        if(!data || data.length < this.postLimit) {
          this.canLoadMore = false;
        }
        console.log(data);
        this.posts.push(...data);
      },
      error: (err : HttpException) =>
        ToastrExtension.handleErrors(this.toastr, err.errors),
      complete: () => {this.loading = false}
    })
  }
}
