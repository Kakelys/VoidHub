import { TranslateService } from '@ngx-translate/core'
import { ToastrService } from 'ngx-toastr'
import { Observable, catchError, forkJoin, switchMap, throwError } from 'rxjs'

import {
    HttpErrorResponse,
    HttpEvent,
    HttpHandler,
    HttpInterceptor,
    HttpRequest,
} from '@angular/common/http'
import { Injectable } from '@angular/core'

import { HttpException } from '../models/http-exception.model'

@Injectable()
export class HttpExceptionInterceptor implements HttpInterceptor {
    constructor(
        private toastr: ToastrService,
        private trans: TranslateService
    ) {}

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError((err) => {
                const errors = []
                //TODO: mb simply try instant instead of get?
                const subs = []
                if (err instanceof HttpErrorResponse) {
                    console.error(err)
                    switch (err.status) {
                        case 400:
                            this.handle400(err, errors, subs)
                            break
                        case 401:
                            errors.push(err.statusText)
                            break
                        case 403:
                            this.handle403(err, errors, subs)
                            break
                        case 404:
                            this.handle404(err, errors, subs)
                            break
                        case 500:
                            this.handle500(errors, subs)
                            break
                        default:
                            this.handleDefault(err, errors, subs)
                            break
                    }
                }

                if (err instanceof Error) {
                    errors.push(err.message)
                }

                if (subs.length == 0) {
                    const httpException: HttpException = {
                        statusCode: err.status,
                        errors: errors,
                        error: err,
                    }

                    return throwError(() => httpException)
                }

                return forkJoin(subs).pipe(
                    switchMap((val) => {
                        const httpException: HttpException = {
                            statusCode: err.status,
                            errors: errors,
                            error: err,
                        }

                        return throwError(() => httpException)
                    })
                )
            })
        )
    }

    handle400(err, errors, subs) {
        if (err.error.errors) {
            //check if err.error.errors is array
            if (Array.isArray(err.error.errors)) {
                const validErrors = err.error.errors
                for (const element of validErrors) errors.push(element.ErrorMessage)
            } else {
                Object.keys(err.error.errors).forEach((key) => {
                    errors.push(err.error.errors[key][0])
                })
            }

            return
        }

        if (err.error) {
            errors.push(err.error)
        }
    }

    handle403(err, errors, subs) {
        if (err.error?.ExpiresAt && err.error?.Reason) {
            const banReason$ = this.trans.get('labels.you-banned-until').pipe(
                switchMap((bannedLabel) => {
                    return this.trans.get('labels.reason').pipe(
                        switchMap((reasonLabel) => {
                            const msg = `${bannedLabel} ${new Date(err.error.ExpiresAt).toLocaleString()}, ${reasonLabel}: ${err.error.Reason}`

                            errors.push(msg)
                            this.toastr.error(msg)
                            return reasonLabel
                        })
                    )
                })
            )

            subs.push(banReason$)
        }

        const accessDenied$ = this.trans.get('labels.access-denied').pipe(
            switchMap((label) => {
                errors.push(label)
                return label
            })
        )

        subs.push(accessDenied$)
    }

    handle404(err, errors, subs) {
        if (err.error && err.error.indexOf('<html') == -1) {
            errors.push(err.error)
        } else {
            const notFound$ = this.trans.get('labels.fail-request').pipe(
                switchMap((label) => {
                    errors.push(label)
                    return label
                })
            )

            subs.push(notFound$)
        }
    }

    handle500(errors, subs) {
        const internalError$ = this.trans.get('lables.internal-error').pipe(
            switchMap((label) => {
                errors.push(label)
                return label
            })
        )

        subs.push(internalError$)
    }

    handleDefault(err, errors, subs) {
        // connection refused or timeout
        if (err.status == 0) {
            const connectionRefused$ = this.trans.get('labels.connection-refused').pipe(
                switchMap((label) => {
                    errors.push(label)
                    return label
                })
            )

            subs.push(connectionRefused$)

            return
        }

        const unknownError$ = this.trans.get('labels.something-went-wrong').pipe(
            switchMap((label) => {
                errors.push(label)
                return label
            })
        )

        subs.push(unknownError$)
    }
}
