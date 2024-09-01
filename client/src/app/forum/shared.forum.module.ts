import { TranslateModule } from '@ngx-translate/core'

import { NgModule } from '@angular/core'

import { SharedEditorModule } from 'src/shared/shared-editor.module'
import { SharedModule } from 'src/shared/shared.module'

import { CommentsComponent } from './post/comments/comments.component'
import { PostEditorComponent } from './post/post-editor/post-editor.component'
import { PostElementComponent } from './post/post-element/post-element.component'
import { UploadImagesComponent } from './post/upload-images/upload-images.component'
import { RecentElementComponent } from './recent/recent-element/recent-element.component'
import { ReducePost } from './recent/reduce-post.pipe'
import { PostService } from './services/post.service'
import { TopicService } from './services/topic.service'
import { UploadService } from './services/upload.service'
import { DeleteComponent } from './shared/delete/delete.component'

@NgModule({
    providers: [PostService, UploadService, TopicService],
    declarations: [
        CommentsComponent,
        PostEditorComponent,
        UploadImagesComponent,
        PostElementComponent,
        RecentElementComponent,
    ],
    imports: [
        DeleteComponent,
        SharedEditorModule,
        SharedModule,
        TranslateModule.forChild(),
        ReducePost,
    ],
    exports: [
        CommentsComponent,
        PostEditorComponent,
        UploadImagesComponent,
        PostElementComponent,
        RecentElementComponent,
    ],
})
export class SharedForumModule {}
