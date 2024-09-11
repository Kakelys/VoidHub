import { TranslateModule } from '@ngx-translate/core'

import { NgModule } from '@angular/core'
import { RouterModule } from '@angular/router'

import { SharedModule } from '../shared.module'

import { SharedEditorModule } from '../shared-editor.module'
import { SharedForumModule } from './shared.forum.module'
import { TruncatePipe } from 'src/shared/truncate.pipe'

import {
    BreadcrumbComponent,
    DeleteComponent,
    ErrorMessageListComponent,
    PaginatorComponent,
    PaginatorV2Component,
} from 'src/app/common/components'

import { AdminService, BanService, canActivateAdmin } from '../admin'
import {
    ClosedIconComponent,
    ForumComponent,
    ForumElementComponent,
    ForumListComponent,
    MainComponent,
    NewForumComponent,
    NewSectionComponent,
    NewTopicComponent,
    NewTopicPageComponent,
    RecentComponent,
    SectionElementComponent,
    SectionListComponent,
    TitleEditorComponent,
    TopicComponent,
    TopicElementComponent,
} from './components'
import { PinnedIconComponent } from './components/pinned-icon/pinned-icon.component'
import { ReducePost } from './pipes'
import {
    AccountService,
    BreadcrumbService,
    ForumService,
    NameService,
    SectionService,
} from './services'

@NgModule({
    providers: [
        SectionService,
        ForumService,
        BanService,
        AdminService,
        AccountService,
        NameService,
        BreadcrumbService,
    ],
    declarations: [
        SectionListComponent,
        SectionElementComponent,
        ForumListComponent,
        MainComponent,
        TopicComponent,
        NewSectionComponent,
        NewForumComponent,
        NewTopicComponent,
        TitleEditorComponent,
        ForumComponent,
        ForumElementComponent,
        RecentComponent,
        BreadcrumbComponent,
        NewTopicPageComponent,
    ],
    imports: [
        SharedModule,
        SharedEditorModule,
        SharedForumModule,
        PinnedIconComponent,
        ClosedIconComponent,
        TranslateModule,
        DeleteComponent,
        ErrorMessageListComponent,
        PaginatorComponent,
        PaginatorV2Component,
        TopicElementComponent,
        ReducePost,
        TruncatePipe,
        RouterModule.forChild([
            {
                path: '',
                component: MainComponent,
                children: [
                    { path: '', redirectTo: 'sections', pathMatch: 'full' },
                    { path: 'sections', component: SectionListComponent },
                    { path: 'new-section', component: NewSectionComponent },
                    { path: 'new-topic', component: NewTopicPageComponent },
                    { path: 'recent', component: RecentComponent },
                    { path: 'section/:id/new-forum', component: NewForumComponent },
                    { path: 'topic/:id/:page', component: TopicComponent },
                    { path: 'topic/:id', redirectTo: 'topic/:id/1', pathMatch: 'full' },
                    {
                        path: 'search',
                        loadChildren: () =>
                            import('../search/search.module').then((m) => m.SearchModule),
                    },
                    {
                        path: 'admin-panel',
                        loadChildren: () =>
                            import('../admin/admin-panel.module').then((m) => m.AdminPanelModule),
                    },
                    { path: ':id', component: ForumComponent },
                    {
                        path: ':id/deleted',
                        component: ForumComponent,
                        canActivate: [canActivateAdmin],
                    },
                ],
            },
        ]),
        PaginatorV2Component,
    ],
    exports: [],
})
export class ForumModule {}
