import { Component } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { PostElementComponent } from 'src/app/forum/post/post-element/post-element.component';
import { PostService } from 'src/app/forum/services/post.service';

@Component({
  selector: 'app-account-post-element',
  templateUrl: './element.component.html',
  styleUrls: ['./element.component.css']
})
export class AccountPostElementComponent extends PostElementComponent {

  constructor(postService: PostService,
    toastr: ToastrService) {
    super(postService, toastr);
  }

}
