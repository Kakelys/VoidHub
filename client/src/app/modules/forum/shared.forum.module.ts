import { TranslateModule } from '@ngx-translate/core'

import { NgModule } from '@angular/core'

import { SharedModule } from '../shared.module'

import { SharedEditorModule } from '../shared-editor.module'

import { DeleteComponent } from 'src/app/common/components'

import {
    CommentsComponent,
    PostEditorComponent,
    PostElementComponent,
    RecentElementComponent,
    UploadImagesComponent,
} from './components'
import { ReducePost } from './pipes/reduce-post.pipe'
import { PostService, TopicService, UploadService } from './services'

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
