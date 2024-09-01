import { TranslateModule } from '@ngx-translate/core'

import { NgModule } from '@angular/core'

import { SharedModule } from 'src/shared/shared.module'

import { ErrorMessageListComponent } from 'src/app/utils/error-message-list/error-message-list.component'

import { AccountService } from '../services/account.service'
import { AdminPanelComponent } from './admin-panel/admin-panel.component'
import { BanMenuComponent } from './ban-menu/ban-menu.component'
import { DefaultAvatarComponent } from './default-avatar/default-avatar.component'
import { RenameMenuComponent } from './rename-menu/rename-menu.component'
import { RoleMenuComponent } from './role-menu/role-menu.component'
import { AdminService } from './services/admin.service'
import { BanService } from './services/ban.service'

@NgModule({
    declarations: [
        BanMenuComponent,
        RoleMenuComponent,
        AdminPanelComponent,
        DefaultAvatarComponent,
        RenameMenuComponent,
    ],
    imports: [SharedModule, ErrorMessageListComponent, TranslateModule.forChild()],
    providers: [AccountService, BanService, AdminService],
    exports: [
        BanMenuComponent,
        RoleMenuComponent,
        AdminPanelComponent,
        DefaultAvatarComponent,
        RenameMenuComponent,
    ],
})
export class AdminComponentsModule {}
