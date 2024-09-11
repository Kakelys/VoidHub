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

import { AppRoutingModule } from './app-routing.module'
import { AuthService } from './modules/auth'
import { AuthModule } from './modules/auth/auth.module'
import { AuthInterceptor } from './modules/auth/interceptors/auth.interceptor'
import { SectionService } from './modules/forum'
import { SearchBarComponent } from './modules/search'
import { SharedModule } from './modules/shared.module'

import { environment } from 'src/environments/environment'
import { LocalizeInterceptor } from 'src/shared'

import { AppComponent } from './app.component'
import {
    GeneralStatsComponent,
    HeaderComponent,
    HomeComponent,
    HttpExceptionInterceptor,
    LimiterInterceptor,
    LimiterService,
    MenuComponent,
    NewMessageComponent,
    NewMessageListener,
    NotifyService,
    OnlineStatsComponent,
    RightMenuComponent,
    SignalrService,
    StatsService,
} from './common'

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
