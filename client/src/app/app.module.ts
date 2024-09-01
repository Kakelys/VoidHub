import { TranslateLoader, TranslateModule, TranslateService } from '@ngx-translate/core'
import { TranslateHttpLoader } from '@ngx-translate/http-loader'
import { ToastrModule, ToastrService } from 'ngx-toastr'

import { HashLocationStrategy, LocationStrategy, registerLocaleData } from '@angular/common'
import { HTTP_INTERCEPTORS, HttpClient, HttpClientModule } from '@angular/common/http'
import ruLocale from '@angular/common/locales/ru'
import { APP_INITIALIZER, LOCALE_ID, NgModule } from '@angular/core'
import { FormsModule } from '@angular/forms'
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'
import { RouterModule } from '@angular/router'

import { HttpExceptionInterceptor } from 'src/shared/error/http-exception.interceptor'
import { LocalizeInterceptor } from 'src/shared/localize.interceptor'
import { SharedModule } from 'src/shared/shared.module'
import { SignalrService } from 'src/shared/signalr.service'

import { environment } from 'src/environments/environment'

import { AppRoutingModule } from './app-routing.module'
import { AppComponent } from './app.component'
import { AuthInterceptor } from './auth/auth.interceptor'
import { AuthModule } from './auth/auth.module'
import { AuthService } from './auth/auth.service'
import { SearchBarComponent } from './forum/search/search-bar/search-bar.component'
import { SectionService } from './forum/services/section.service'
import { HeaderComponent } from './header/header.component'
import { HomeComponent } from './home/home.component'
import { MenuComponent } from './menu/left-menu/menu.component'
import { RightMenuComponent } from './menu/right-menu/right-menu.component'
import { NewMessageComponent } from './notify/new-message/new-message.component'
import { NewMessageListener } from './notify/new-message/new-message.listener'
import { NotifyService } from './notify/notify.service'
import { LimiterInterceptor } from './utils/limitter/limitter.interceptor'
import { LimiterService } from './utils/limitter/limitter.service'
import { GeneralStatsComponent } from './utils/stats/general-stats/general-stats.component'
import { OnlineStatsComponent } from './utils/stats/online-stats/online-stats.component'
import { StatsService } from './utils/stats/stats.service'

export function HttpLoaderFactory(http: HttpClient) {
    return new TranslateHttpLoader(http, environment.localizationPrefix, '.json')
}

registerLocaleData(ruLocale)

@NgModule({
    declarations: [
        AppComponent,
        HeaderComponent,
        HomeComponent,
        NewMessageComponent,
        MenuComponent,
        RightMenuComponent,
        OnlineStatsComponent,
        GeneralStatsComponent,
    ],
    imports: [
        SharedModule,
        HttpClientModule,
        AppRoutingModule,
        TranslateModule.forRoot({
            loader: {
                provide: TranslateLoader,
                useFactory: HttpLoaderFactory,
                deps: [HttpClient],
            },
        }),
        FormsModule,
        AuthModule,
        SearchBarComponent,
        BrowserAnimationsModule,
        ToastrModule.forRoot({
            preventDuplicates: true,
            countDuplicates: true,
        }),
        RouterModule.forChild([{ path: '**', component: HomeComponent, pathMatch: 'full' }]),
    ],
    providers: [
        LimiterService,
        SignalrService,
        NotifyService,
        SectionService,
        StatsService,
        NewMessageListener,
        {
            provide: LocationStrategy,
            useClass: HashLocationStrategy,
        },
        {
            provide: LOCALE_ID,
            useFactory: () => localStorage.getItem('locale') ?? 'en',
        },
        {
            provide: APP_INITIALIZER,
            // eslint-disable-next-line @typescript-eslint/no-empty-function, @typescript-eslint/no-unused-vars
            useFactory: (ns: NotifyService, nml: NewMessageListener) => () => {},
            deps: [NotifyService, NewMessageListener],
            multi: true,
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: HttpExceptionInterceptor,
            deps: [ToastrService, TranslateService],
            multi: true,
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: LimiterInterceptor,
            deps: [LimiterService],
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
    bootstrap: [AppComponent],
})
export class AppModule {}
