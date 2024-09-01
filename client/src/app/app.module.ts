import { BrowserAnimationsModule } from '@angular/platform-browser/animations'
import { registerLocaleData, LocationStrategy, HashLocationStrategy } from '@angular/common'
import { HttpClientModule, HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http'
import ruLocale from '@angular/common/locales/ru'
import { NgModule, LOCALE_ID, APP_INITIALIZER } from '@angular/core'
import { FormsModule } from '@angular/forms'
import { RouterModule } from '@angular/router'

import { TranslateModule, TranslateLoader, TranslateService } from '@ngx-translate/core'
import { TranslateHttpLoader } from '@ngx-translate/http-loader'
import { ToastrModule, ToastrService } from 'ngx-toastr'
import { environment } from 'src/environments/environment'
import { LocalizeInterceptor } from 'src/shared'
import { AppRoutingModule } from './app-routing.module'
import { AppComponent } from './app.component'
import {
    HeaderComponent,
    HomeComponent,
    NewMessageComponent,
    MenuComponent,
    RightMenuComponent,
    OnlineStatsComponent,
    GeneralStatsComponent,
    LimiterService,
    SignalrService,
    NotifyService,
    StatsService,
    NewMessageListener,
    HttpExceptionInterceptor,
    LimiterInterceptor,
} from './common'
import { AuthService } from './modules/auth'
import { AuthInterceptor } from './modules/auth/interceptors/auth.interceptor'
import { SectionService } from './modules/forum'
import { SearchBarComponent } from './modules/search'
import { SharedModule } from './modules/shared.module'
import { AuthModule } from './modules/auth/auth.module'

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
