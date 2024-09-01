import { TranslateModule } from '@ngx-translate/core'

import { NgModule } from '@angular/core'
import { RouterModule } from '@angular/router'

import { SharedModule } from 'src/shared/shared.module'

import { AdminComponentsModule } from './admin-components.module'
import { AdminPanelComponent } from './admin-panel/admin-panel.component'
import { BanMenuComponent } from './ban-menu/ban-menu.component'
import { RenameMenuComponent } from './rename-menu/rename-menu.component'
import { RoleMenuComponent } from './role-menu/role-menu.component'
import { canActivateAdmin } from './role.guard'
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
