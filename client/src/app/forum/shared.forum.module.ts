import { NgModule } from "@angular/core";
import { CommentsComponent } from "./post/comments/comments.component";
import { PostEditorComponent } from "./post/post-editor/post-editor.component";
import { UploadImagesComponent } from "./post/upload-images/upload-images.component";
import { PostService } from "./services/post.service";
import { DeleteComponent } from "./shared/delete/delete.component";
import { PostElementComponent } from "./post/post-element/post-element.component";
import { SharedModule } from "src/shared/shared.module";
import { SharedEditorModule } from "src/shared/shared-editor.module";
import { UploadService } from "./services/upload.service";
import { TopicService } from "./services/topic.service";


@NgModule({
  providers: [
    PostService,
    UploadService,
    TopicService,
  ],
  declarations: [
    CommentsComponent,
    PostEditorComponent,
    UploadImagesComponent,
    PostElementComponent,
  ],
  imports: [
    DeleteComponent,
    SharedEditorModule,
    SharedModule
  ],
  exports: [
    CommentsComponent,
    PostEditorComponent,
    UploadImagesComponent,
    PostElementComponent,
  ]
})
export class SharedForumModule {}
