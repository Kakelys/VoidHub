import { TranslateModule } from '@ngx-translate/core'

import { NgModule } from '@angular/core'
import { RouterModule } from '@angular/router'

import { SharedModule } from '../shared.module'

import { AdminComponentsModule } from './admin-components.module'
import {
    AdminPanelComponent,
    BanMenuComponent,
    RenameMenuComponent,
    RoleMenuComponent,
} from './components'
import { canActivateAdmin } from './guards/role.guard'
import { AdminService } from './services/admin.service'

@NgModule({
    declarations: [],
    imports: [
        SharedModule,
        AdminComponentsModule,
        TranslateModule.forChild(),
        RouterModule.forChild([
            {
                path: '',
                canActivate: [canActivateAdmin],
                component: AdminPanelComponent,
                children: [
                    { path: 'ban-menu', component: BanMenuComponent },
                    { path: 'role-menu', component: RoleMenuComponent },
                    { path: 'rename-menu', component: RenameMenuComponent },
                ],
            },
        ]),
    ],
    providers: [AdminService],
    exports: [],
})
export class AdminPanelModule {}
