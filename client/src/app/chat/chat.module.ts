import { TranslateModule } from '@ngx-translate/core'

import { NgModule } from '@angular/core'
import { RouterModule } from '@angular/router'

import { SharedModule } from 'src/shared/shared.module'

import { ChatMainComponent } from './chat-main/chat-main.component'
import { ChatComponent } from './chat/chat.component'
import { ChatService } from './services/chat.service'

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
