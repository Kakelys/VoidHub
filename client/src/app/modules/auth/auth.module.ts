import { TranslateModule } from '@ngx-translate/core'

import { APP_INITIALIZER, NgModule } from '@angular/core'
import { RouterModule } from '@angular/router'

import { ErrorMessageListComponent } from 'src/app/common/components/error-message-list/error-message-list.component'

import {
    LoginComponent,
    PasswordRecoverComponent,
    PasswordRecoverRequestComponent,
    RegisterComponent,
} from './components'
import { AuthService, RecoverService } from './services'
import { SharedModule } from '../shared.module'

function onAppLoad(authService: AuthService): () => Promise<void> {
    return async () => {
        const user = localStorage.getItem('user')
        if (user) {
            try {
                const parsedUser = JSON.parse(user)
                authService.setUser(parsedUser)
                authService.setRefreshTimeout()
            } catch (err) {
                console.error('Error while trying to auto-login user', err)
            }
        }
    }
}

@NgModule({
    declarations: [
        LoginComponent,
        RegisterComponent,
        PasswordRecoverRequestComponent,
        PasswordRecoverComponent,
    ],
    imports: [
        SharedModule,
        ErrorMessageListComponent,
        TranslateModule.forChild(),
        RouterModule.forChild([
            { path: 'login', component: LoginComponent },
            { path: 'register', component: RegisterComponent },
            { path: 'password-recover', component: PasswordRecoverRequestComponent },
            { path: 'password-recover/:token', component: PasswordRecoverComponent },
        ]),
    ],
    exports: [],
    providers: [
        AuthService,
        RecoverService,
        {
            provide: APP_INITIALIZER,
            useFactory: onAppLoad,
            deps: [AuthService],
            multi: true,
        },
    ],
})
export class AuthModule {}
