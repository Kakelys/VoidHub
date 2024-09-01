import { TranslateModule } from '@ngx-translate/core'

import { NgModule } from '@angular/core'
import { RouterModule } from '@angular/router'

import { SharedModule } from '../shared.module'

import { SharedEditorModule } from '../shared-editor.module'

import { ErrorMessageListComponent, PaginatorComponent } from 'src/app/common/components'

import { SearchService, TopicElementComponent } from '../forum'
import { SearchBarComponent, SearchComponent } from './components'
import { SharedForumModule } from '../forum/shared.forum.module'

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
