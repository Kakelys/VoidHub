import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ToastrService } from "ngx-toastr";
import { Observable, catchError, forkJoin, lastValueFrom, switchMap, throwError } from "rxjs";
import { HttpException } from "../models/http-exception.model";
import { TranslateService } from "@ngx-translate/core";

@Injectable()
export class HttpExceptionInterceptor implements HttpInterceptor {

  constructor(
    private toastr: ToastrService,
    private trans: TranslateService
  ){}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((err) => {
        let errors = [];
        //TODO: mb simply try instant instead of get?
        let subs = [];
        if (err instanceof HttpErrorResponse) {
          console.error(err);
          switch (err.status) {
            case 400:
              if (err.error.errors) {
                //check if err.error.errors is array
                if (Array.isArray(err.error.errors)) {
                  let validErrors = err.error.errors;
                  for(let i = 0; i < validErrors.length; i++)
                    errors.push(validErrors[i].ErrorMessage);
                  }
                else {
                  Object.keys(err.error.errors).forEach(key => {
                    errors.push(err.error.errors[key][0]);
                  });
                }

                break;
              }

              if (err.error) {
                errors.push(err.error)
                break;
              }

              break;
            case 401:
              errors.push(err.statusText)

              break;
            case 403:
              if(err.error?.ExpiresAt && err.error?.Reason) {
                let banReason$ = this.trans.get('labels.you-banned-until')
                .pipe(switchMap(bannedLabel => {
                  return this.trans.get('labels.reason')
                  .pipe(switchMap(reasonLabel => {
                    let msg = `${bannedLabel} ${new Date(err.error.ExpiresAt).toLocaleString()}, ${reasonLabel}: ${err.error.Reason}`;

                    errors.push(msg);
                    this.toastr.error(msg);
                    return reasonLabel;
                  }))
                }))

                subs.push(banReason$);
              }

              let accessDenied$ = this.trans.get('labels.access-denied')
              .pipe(switchMap(label => {
                errors.push(label);
                return label;
              }))

              subs.push(accessDenied$);

              break;
            case 404:

              if(err.error) {
                errors.push(err.error);
              } else {
                let notFound$ = this.trans.get('labels.fail-request')
                .pipe(switchMap(label => {
                  errors.push(label);
                  return label;
                }))

                subs.push(notFound$);
              }

              break;
            case 500:
              let internalError$ = this.trans.get('lables.internal-error')
              .pipe(switchMap(label => {
                errors.push(label);
                return label;
              }))

              subs.push(internalError$);

              break;
            default:
              // connection refused or timeout
              if(err.status == 0) {
                let connectionRefused$ = this.trans.get('labels.connection-refused')
                .pipe(switchMap(label => {
                  errors.push(label);
                  return label;
                }))

                subs.push(connectionRefused$);

                break;
              }

              let unknownError$ = this.trans.get('labels.something-went-wrong')
              .pipe(switchMap(label => {
                errors.push(label);
                return label;
              }))

              subs.push(unknownError$);

              break;
          }
        }

        if(err instanceof Error) {
          errors.push(err.message);
        }

        return forkJoin(subs).pipe(switchMap(val => {
          const httpException: HttpException = {
            statusCode: err.status,
            errors: errors,
            error: err
          }

          return throwError(() => httpException);
        }))
      })
    );
  }
}
