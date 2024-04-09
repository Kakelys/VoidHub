import { APP_INITIALIZER, NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HTTP_INTERCEPTORS, HttpClient, HttpClientModule} from '@angular/common/http';
import { HeaderComponent } from './header/header.component';
import { FormsModule } from '@angular/forms';
import { AuthModule } from './auth/auth.module';
import { HomeComponent } from './home/home.component';
import { SharedModule } from 'src/shared/shared.module';
import { AuthInterceptor } from './auth/auth.interceptor';
import { HttpExceptionInterceptor } from 'src/shared/error/http-exception.interceptor';
import { RouterModule } from '@angular/router';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LimitterInterceptor } from 'src/app/limitter/limitter.interceptor';
import { LimitterService } from './limitter/limitter.service';
import { SearchBarComponent } from './forum/search/search-bar/search-bar.component';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { SignalrService } from 'src/shared/signalr.service';
import { NotifyService } from './notify/notify.service';
import { NewMessageListener } from './notify/new-message.listener';
import { NewMessageComponent } from './notify/new-message/new-message.component';
import { MenuComponent } from './menu/menu.component';
import { SectionService } from './forum/services/section.service';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { LocalizeInterceptor } from 'src/shared/localize.interceptor';

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    HomeComponent,
    NewMessageComponent,
    MenuComponent,
  ],
  imports: [
    SharedModule,
    HttpClientModule,
    AppRoutingModule,
    FormsModule,
    AuthModule,
    SearchBarComponent,
    BrowserAnimationsModule,
    TranslateModule.forRoot({
      defaultLanguage: 'en',
      loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory,
          deps: [HttpClient]
      }
    }),
    ToastrModule.forRoot({
      preventDuplicates: true,
      countDuplicates: true,
    }),
    RouterModule.forChild([
      {path: '**', component: HomeComponent, pathMatch: 'full'},
    ])
  ],
  providers: [
    LimitterService,
    SignalrService,
    NotifyService,
    SectionService,
    NewMessageListener,
    {
      provide: LocationStrategy,
      useClass: HashLocationStrategy
    },
    {
      provide: APP_INITIALIZER,
      useFactory: (ns: NotifyService, nml: NewMessageListener) => () => {},
      deps: [NotifyService, NewMessageListener],
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpExceptionInterceptor,
      deps: [ToastrService],
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LimitterInterceptor,
      deps: [LimitterService],
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LocalizeInterceptor,
      multi: true,
    },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
