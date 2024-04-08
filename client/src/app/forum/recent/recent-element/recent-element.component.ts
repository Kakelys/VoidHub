import { Component, Input, OnInit } from '@angular/core';
import { PostElementComponent } from '../../post/post-element/post-element.component';
import { PostService } from '../../services/post.service';
import { ToastrService } from 'ngx-toastr';
import { TopicInfo } from '../../models/topic-info.model';

@Component({
  selector: 'app-recent-element',
  templateUrl: './recent-element.component.html',
  styleUrls: ['./recent-element.component.css']
})
export class RecentElementComponent extends PostElementComponent{

  @Input()
  topicInfo: TopicInfo;

  constructor(postService: PostService, toastr: ToastrService) {
    super(postService, toastr)
  }

}
