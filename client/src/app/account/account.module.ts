import { NgModule } from "@angular/core";
import { ProfileComponent } from "./profile/profile.component";
import { SettingsComponent } from "./settings/settings.component";
import { AccountService } from "./account.service";
import { SharedModule } from "src/shared/shared.module";
import { RouterModule } from "@angular/router";
import { canActivateSelf } from "./self.guard";
import { AdminComponentsModule } from "../forum/admin/admin-components.module";
import { BanMenuComponent } from "../forum/admin/ban-menu/ban-menu.component";
import { RoleMenuComponent } from "../forum/admin/role-menu/role-menu.component";
import { canActivateAdmin, canActivateModer } from "../forum/admin/role.guard";
import { ErrorMessageListComponent } from "../error-message-list/error-message-list.component";
import { RenameMenuComponent } from "../forum/admin/rename-menu/rename-menu.component";
import { ChatService } from "../chat/services/chat.service";
import { AccountTopicsComponent } from './topics/topics.component';
import { AccountPostsComponent } from './posts/posts.component';
import { SharedEditorModule } from "src/shared/shared-editor.module";
import { AccountPostElementComponent } from './posts/element/element.component';
import { PostService } from "../forum/services/post.service";
import { SharedForumModule } from "../forum/shared.forum.module";
import { AccountTopicElement } from './topics/element/element.component';
import { TranslateModule } from "@ngx-translate/core";

@NgModule({
  declarations: [
    ProfileComponent,
    SettingsComponent,
    AccountTopicsComponent,
    AccountPostsComponent,
    AccountPostElementComponent,
    AccountTopicElement
  ],
  imports: [
    SharedModule,
    SharedEditorModule,
    SharedForumModule,
    AdminComponentsModule,
    TranslateModule.forChild(),
    ErrorMessageListComponent,
    RouterModule.forChild([
      {path: 'settings', component: SettingsComponent, canActivate: [canActivateSelf]},
      {path: '', component: ProfileComponent},
      {path: ':id', component: ProfileComponent, children: [
        {path: 'topics', component: AccountTopicsComponent},
        {path: 'posts', component: AccountPostsComponent},
        {path: 'ban-menu', component: BanMenuComponent, canActivate: [canActivateAdmin]},
        {path: 'role-menu', component: RoleMenuComponent, canActivate: [canActivateAdmin]},
        {path: 'rename-menu', component: RenameMenuComponent, canActivate: [canActivateModer]},
      ]},
    ])
  ],
  providers: [
    AccountService,
    ChatService,
    PostService
  ]
})
export class AccountModule {}
