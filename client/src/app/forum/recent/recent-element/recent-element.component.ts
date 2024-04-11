import { Component, Input } from '@angular/core';
import { PostElementComponent } from '../../post/post-element/post-element.component';
import { PostService } from '../../services/post.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-recent-element',
  templateUrl: './recent-element.component.html',
  styleUrls: ['./recent-element.component.css']
})
export class RecentElementComponent extends PostElementComponent{

  @Input()
  symbolLimit: number = 200;

  @Input()
  tagLimit: number = 11;

  constructor(postService: PostService, toastr: ToastrService) {
    super(postService, toastr)
  }

}
