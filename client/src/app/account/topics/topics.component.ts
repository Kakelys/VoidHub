import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import Editor from 'ckeditor5/build/ckeditor';
import { ReplaySubject, debounceTime, fromEvent, takeUntil } from 'rxjs';
import { environment } from 'src/environments/environment';
import { AccountService } from '../account.service';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { TopicInfo } from 'src/app/forum/models/topic-info.model';
import { Offset } from 'src/shared/offset.model';
import { HttpException } from 'src/shared/models/http-exception.model';
import { ToastrExtension } from 'src/shared/toastr.extension';

@Component({
  selector: 'app-topics',
  templateUrl: './topics.component.html',
  styleUrls: ['./topics.component.css']
})
export class AccountTopicsComponent implements OnInit, OnDestroy {

  editor = Editor as {create: any}
  topics: TopicInfo[] = []

  loadTime = new Date();
  postLimit = 10;
  canLoadMore = true;
  loading = false;
  id: number;

  limitNames = environment.limitNames;
  resourceUrl = environment.resourceURL;

  @ViewChild('topicsContainer', {static: true})
  topicsContainer: ElementRef;

  private destroy$ = new ReplaySubject<boolean>(1);

  constructor(
    private accService: AccountService,
    private route: ActivatedRoute,
    private toastr: ToastrService) {

  }

  ngOnInit(): void {
    this.route.parent.params.subscribe(params => {
      this.handleId(params['id']);
    })

    fromEvent(window, 'scroll')
    .pipe(takeUntil(this.destroy$), debounceTime(300))
    .subscribe((e:any) => {
      const currentScroll = e.target.documentElement.scrollTop + e.target.documentElement.clientHeight;
      const blockEnd = this.topicsContainer.nativeElement.offsetHeight + this.topicsContainer.nativeElement.offsetTop;

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
      this.toastr.error("Wrong account id");
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

    const offset = new Offset(this.topics.length, this.postLimit);

    this.accService.getTopics(this.id, this.loadTime, offset)
    .subscribe({
      next: (data: TopicInfo[]) => {
        if(!data || data.length < this.postLimit) {
          this.canLoadMore = false;
        }

        this.topics.push(...data);
      },
      error: (err : HttpException) =>
        ToastrExtension.handleErrors(this.toastr, err.errors),
      complete: () => {this.loading = false}
    })
  }
}
