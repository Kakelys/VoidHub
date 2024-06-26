import { AuthService } from './auth/auth.service';
import { APP_INITIALIZER, LOCALE_ID, NgModule } from '@angular/core';

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
import { SearchBarComponent } from './forum/search/search-bar/search-bar.component';
import { HashLocationStrategy, LocationStrategy, registerLocaleData } from '@angular/common';
import { SignalrService } from 'src/shared/signalr.service';
import { NotifyService } from './notify/notify.service';
import { NewMessageComponent } from './notify/new-message/new-message.component';
import { MenuComponent } from './menu/left-menu/menu.component';
import { SectionService } from './forum/services/section.service';
import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { LocalizeInterceptor } from 'src/shared/localize.interceptor';
import { environment } from 'src/environments/environment';
import ruLocale from '@angular/common/locales/ru';
import { RightMenuComponent } from './menu/right-menu/right-menu.component';
import { LimitterInterceptor } from './utils/limitter/limitter.interceptor';
import { LimitterService } from './utils/limitter/limitter.service';
import { OnlineStatsComponent } from './utils/online-stats/online-stats.component';
import { NewMessageListener } from './notify/new-message/new-message.listener';

export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, environment.localizationPrefix, '.json');
}

registerLocaleData(ruLocale);

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    HomeComponent,
    NewMessageComponent,
    MenuComponent,
    RightMenuComponent,
    OnlineStatsComponent,
  ],
  imports: [
    SharedModule,
    HttpClientModule,
    AppRoutingModule,
    TranslateModule.forRoot({
      loader: {
          provide: TranslateLoader,
          useFactory: HttpLoaderFactory,
          deps: [HttpClient]
      }
    }),
    FormsModule,
    AuthModule,
    SearchBarComponent,
    BrowserAnimationsModule,
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
      provide: LOCALE_ID,
      useFactory: () => localStorage.getItem('locale') ?? 'en'},
    {
      provide: APP_INITIALIZER,
      useFactory: (ns: NotifyService, nml: NewMessageListener) => () => {},
      deps: [NotifyService, NewMessageListener],
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: HttpExceptionInterceptor,
      deps: [ToastrService, TranslateService],
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
      deps: [AuthService, ToastrService, TranslateService],
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
