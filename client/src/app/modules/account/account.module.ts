import { NgModule } from '@angular/core'
import { RouterModule } from '@angular/router'
import { TranslateModule } from '@ngx-translate/core'
import { ErrorMessageListComponent } from 'src/app/common'
import {
    BanMenuComponent,
    canActivateAdmin,
    RoleMenuComponent,
    RenameMenuComponent,
    canActivateModer,
} from '../admin'
import { ChatService } from '../chat/services'
import { AccountService, PostService } from '../forum'
import { SharedEditorModule } from '../shared-editor.module'
import {
    ProfileComponent,
    SettingsComponent,
    AccountTopicsComponent,
    AccountPostsComponent,
    AccountPostElementComponent,
    AccountTopicElementComponent,
    ConfirmEmailComponent,
} from './components'
import { canActivateSelf } from './guards/self.guard'
import { EmailService } from './services'
import { AdminComponentsModule } from '../admin/admin-components.module'
import { SharedForumModule } from '../forum/shared.forum.module'
import { SharedModule } from '../shared.module'

@NgModule({
    declarations: [
        ProfileComponent,
        SettingsComponent,
        AccountTopicsComponent,
        AccountPostsComponent,
        AccountPostElementComponent,
        AccountTopicElementComponent,
        ConfirmEmailComponent,
    ],
    imports: [
        SharedModule,
        SharedEditorModule,
        SharedForumModule,
        ErrorMessageListComponent,
        AdminComponentsModule,
        TranslateModule.forChild(),
        RouterModule.forChild([
            { path: 'settings', component: SettingsComponent, canActivate: [canActivateSelf] },
            { path: 'confirm-email/:token', component: ConfirmEmailComponent },
            { path: '', component: ProfileComponent },
            {
                path: ':id',
                component: ProfileComponent,
                children: [
                    { path: 'topics', component: AccountTopicsComponent },
                    { path: 'posts', component: AccountPostsComponent },
                    {
                        path: 'ban-menu',
                        component: BanMenuComponent,
                        canActivate: [canActivateAdmin],
                    },
                    {
                        path: 'role-menu',
                        component: RoleMenuComponent,
                        canActivate: [canActivateAdmin],
                    },
                    {
                        path: 'rename-menu',
                        component: RenameMenuComponent,
                        canActivate: [canActivateModer],
                    },
                ],
            },
        ]),
    ],
    providers: [AccountService, ChatService, PostService, EmailService],
})
export class AccountModule {}
