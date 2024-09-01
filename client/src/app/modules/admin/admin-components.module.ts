import { TranslateModule } from '@ngx-translate/core'

import { NgModule } from '@angular/core'

import { SharedModule } from '../shared.module'

import { ErrorMessageListComponent } from 'src/app/common/components'

import { AccountService } from '../forum/services'
import {
    AdminPanelComponent,
    BanMenuComponent,
    DefaultAvatarComponent,
    RenameMenuComponent,
    RoleMenuComponent,
} from './components'
import { AdminService, BanService } from './services'

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
