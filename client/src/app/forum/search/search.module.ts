import { TranslateModule } from '@ngx-translate/core'

import { NgModule } from '@angular/core'
import { RouterModule } from '@angular/router'

import { SharedEditorModule } from 'src/shared/shared-editor.module'
import { SharedModule } from 'src/shared/shared.module'

import { ErrorMessageListComponent } from 'src/app/utils/error-message-list/error-message-list.component'

import { PaginatorComponent } from '../paginator/paginator.component'
import { SearchService } from '../services/search.service'
import { SharedForumModule } from '../shared.forum.module'
import { TopicElementComponent } from '../topic/topic-element/topic-element.component'
import { SearchBarComponent } from './search-bar/search-bar.component'
import { SearchComponent } from './search/search.component'

@NgModule({
    declarations: [SearchComponent],
    imports: [
        SharedModule,
        SharedEditorModule,
        SharedForumModule,
        TranslateModule.forChild(),
        SearchBarComponent,
        ErrorMessageListComponent,
        PaginatorComponent,
        TopicElementComponent,
        RouterModule.forChild([{ path: '', component: SearchComponent }]),
    ],
    providers: [SearchService],
    exports: [],
})
export class SearchModule {}
