import { NgModule } from "@angular/core";
import { ForumListComponent } from "./forum/forum-list/forum-list.component";
import { ForumElementComponent } from "./forum/forum-element/forum-element.component";
import { SectionListComponent } from "./section/section-list/section-list.component";
import { SectionElementComponent } from "./section/section-element/section-element.component";
import { TopicElementComponent } from "./topic/topic-element/topic-element.component";
import { RouterModule } from "@angular/router";
import { MainComponent } from './main/main.component';
import { TopicComponent } from './topic/topic/topic.component';
import { NewSectionComponent } from './section/new-section/new-section.component';
import { SharedModule } from "src/shared/shared.module";
import { ErrorMessageListComponent } from "../utils/error-message-list/error-message-list.component";
import { SectionService } from "./services/section.service";
import { NewForumComponent } from './forum/new-forum/new-forum.component';
import { ForumService } from "./services/forum.service";
import { NewTopicComponent } from './topic/new-topic/new-topic.component';
import { PaginatorComponent } from './paginator/paginator.component';
import { DeleteComponent } from './shared/delete/delete.component';
import { TitleEditorComponent } from './title-editor/title-editor.component';
import { ForumComponent } from "./forum/forum/forum.component";
import { PinnedIconComponent } from './shared/pinned-icon/pinned-icon.component';
import { ClosedIconComponent } from './shared/closed-icon/closed-icon.component';
import { AdminService } from "./admin/services/admin.service";
import { BanService } from "./admin/services/ban.service";
import { AccountService } from "./services/account.service";
import { RecentComponent } from "./recent/recent.component";
import { ReducePost } from "./recent/reduce-post.pipe";
import { TruncatePipe } from "src/shared/truncate.pipe";
import { NameService } from "./services/name.service";
import { SharedEditorModule } from "src/shared/shared-editor.module";
import { BreadcrumbComponent } from './breadcrumb/breadcrumb.component';
import { BreadcrumbService } from "./services/breadcrumb.service";
import { SharedForumModule } from "./shared.forum.module";
import { canActivateAdmin } from "./admin/role.guard";
import { TranslateModule } from "@ngx-translate/core";
import { NewTopicPageComponent } from "./topic/new-topic-page/new-topic-page.component";

@NgModule({
  providers: [
    SectionService,
    ForumService,
    BanService,
    AdminService,
    AccountService,
    NameService,
    BreadcrumbService
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
    NewTopicPageComponent
  ],
  imports: [
    SharedModule,
    SharedEditorModule,
    SharedForumModule,
    TranslateModule,
    DeleteComponent,
    ErrorMessageListComponent,
    PaginatorComponent,
    TopicElementComponent,
    PinnedIconComponent,
    ClosedIconComponent,
    ReducePost,
    TruncatePipe,
    RouterModule.forChild([
        {path: '', component: MainComponent, children: [
          {path: '', redirectTo: 'sections', pathMatch: 'full'},
          {path:'sections', component: SectionListComponent},
          {path:'new-section', component: NewSectionComponent},
          {path:'new-topic', component: NewTopicPageComponent},
          {path:'recent', component: RecentComponent},
          {path:'section/:id/new-forum', component: NewForumComponent},
          {path:'topic/:id/:page', component: TopicComponent},
          {path:'topic/:id', redirectTo: 'topic/:id/1', pathMatch: 'full'},
          {path:'search', loadChildren: () => import('./search/search.module').then(m => m.SearchModule)},
          {path:'admin-panel', loadChildren: () => import('./admin/admin-panel.module').then(m => m.AdminPanelModule)},
          {path:':id', component: ForumComponent},
          {path:':id/deleted', component: ForumComponent, canActivate: [canActivateAdmin]},
        ]}
    ])
  ],
  exports: [
  ]
})
export class ForumModule{}
