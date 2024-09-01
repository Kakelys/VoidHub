import { NgModule } from '@angular/core'
import { RouterModule, Routes } from '@angular/router'

import { HomeComponent } from './common/components/home/home.component'

const routes: Routes = [
    { path: '', pathMatch: 'full', component: HomeComponent },
    {
        path: 'forum',
        loadChildren: () => import('./modules/forum/forum.module').then((m) => m.ForumModule),
    },
    {
        path: 'profile',
        loadChildren: () => import('./modules/account/account.module').then((m) => m.AccountModule),
    },
    {
        path: 'chats',
        loadChildren: () => import('./modules/chat/chat.module').then((m) => m.ChatModule),
    },
]

@NgModule({
    imports: [RouterModule.forRoot(routes, { useHash: true })],
    exports: [RouterModule],
})
export class AppRoutingModule {}
