import { APP_INITIALIZER, NgModule } from "@angular/core";
import { AuthService } from "./auth.service";
import { LoginComponent } from "./login/login.component";
import { RegisterComponent } from "./register/register.component";
import { RouterModule } from "@angular/router";
import { SharedModule } from "src/shared/shared.module";
import { ErrorMessageListComponent } from "../error-message-list/error-message-list.component";
import { TranslateModule } from "@ngx-translate/core";

function onAppLoad(authService: AuthService): () => Promise<any> {
  return async () => {
    const user = localStorage.getItem('user')
    if(user) {
      try {
        const parsedUser = JSON.parse(user);
        authService.setUser(parsedUser);
        authService.setRefreshTimeout();
      } catch(err) {
        console.error('Error while trying to auto-login user', err)
      }
    }
  };
}

@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent
  ],
  imports: [
    SharedModule,
    ErrorMessageListComponent,
    TranslateModule.forChild(),
    RouterModule.forChild([
      {path: 'login', component: LoginComponent},
      {path: 'register', component: RegisterComponent}
    ])
  ],
  exports: [],
  providers: [
    AuthService,
    {
      provide: APP_INITIALIZER,
      useFactory: onAppLoad,
      deps: [AuthService],
      multi: true
    }
  ],
})
export class AuthModule {}
