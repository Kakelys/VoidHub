import { TranslateModule } from '@ngx-translate/core'

import { NgModule } from '@angular/core'
import { RouterModule } from '@angular/router'

import { SharedModule } from '../shared.module'

import { ChatComponent, ChatMainComponent } from './components'
import { ChatService } from './services'

@NgModule({
    declarations: [ChatMainComponent, ChatComponent],
    imports: [
        SharedModule,
        TranslateModule.forChild(),
        RouterModule.forChild([
            {
                path: '',
                component: ChatMainComponent,
                children: [{ path: ':id', component: ChatComponent }],
            },
        ]),
    ],
    providers: [ChatService],
})
export class ChatModule {}
