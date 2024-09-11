import { TranslateModule } from '@ngx-translate/core'

import { NgModule } from '@angular/core'
import { RouterModule } from '@angular/router'

import { AdminComponentsModule } from '../admin/admin-components.module'
import { SharedForumModule } from '../forum/shared.forum.module'
import { SharedEditorModule } from '../shared-editor.module'
import { SharedModule } from '../shared.module'

import { ErrorMessageListComponent } from 'src/app/common'

import {
    BanMenuComponent,
    RenameMenuComponent,
    RoleMenuComponent,
    canActivateAdmin,
    canActivateModer,
} from '../admin'
import { ChatService } from '../chat/services'
import { PostService } from '../forum'
import {
    AccountPostElementComponent,
    AccountPostsComponent,
    AccountTopicElementComponent,
    AccountTopicsComponent,
    ConfirmEmailComponent,
    ProfileComponent,
    SettingsComponent,
} from './components'
import { canActivateSelf } from './guards/self.guard'
import { AccountService, EmailService } from './services'

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
