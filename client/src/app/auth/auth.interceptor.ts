import { Injectable } from '@angular/core'
import { AuthService } from './auth.service'
import { HttpErrorResponse, HttpHandler, HttpRequest } from '@angular/common/http'
import { catchError, switchMap, take, throwError } from 'rxjs'
import { ToastrService } from 'ngx-toastr'
import { HttpException } from 'src/shared/models/http-exception.model'
import { TranslateService } from '@ngx-translate/core'

@Injectable()
export class AuthInterceptor {
    constructor(
        private authService: AuthService,
        private toastr: ToastrService,
        private trans: TranslateService
    ) {}

    intercept(req: HttpRequest<any>, next: HttpHandler) {
        const accessToken = localStorage.getItem('access-token')
        if (!accessToken && !localStorage.getItem('refresh-token')) {
            if (localStorage.getItem('user')) this.handleLogout()

            return next.handle(req)
        }

        let modifiedReq = req.clone({
            headers: req.headers.append('Authorization', 'Bearer ' + accessToken),
        })

        return next.handle(modifiedReq).pipe(
            catchError((err) => {
                if (err instanceof HttpErrorResponse && err.status === 400) {
                    this.handleLogout()
                }

                // If it's Unauthorized(401) try to refresh tokens and resend request
                if (err instanceof HttpErrorResponse && err.status === 401) {
                    localStorage.removeItem('access-token')
                    if (!localStorage.getItem('refresh-token')) {
                        this.handleLogout()
                        return throwError(err)
                    }

                    return this.authService.refreshAndAuth().pipe(
                        switchMap(() => {
                            const accessToken = localStorage.getItem('access-token')

                            if (!accessToken) {
                                this.handleLogout()
                                return next.handle(req)
                            }

                            modifiedReq = req.clone({
                                headers: req.headers.append(
                                    'Authorization',
                                    'Bearer ' + accessToken
                                ),
                            })

                            return next.handle(modifiedReq)
                        }),
                        catchError((err: HttpException) => {
                            console.error('error while updating', err)
                            if (
                                err.error instanceof HttpErrorResponse &&
                                (err.statusCode == 401 || err.statusCode == 404)
                            ) {
                                this.handleLogout()
                            }

                            return throwError(err)
                        })
                    )
                } else {
                    return throwError(err)
                }
            })
        )
    }

    handleLogout() {
        this.trans
            .get('labels.session-expired')
            .pipe(take(1))
            .subscribe((str) => {
                this.toastr.error(str)
            })

        this.authService.logout()
    }
}
