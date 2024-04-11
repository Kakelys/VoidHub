import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ToastrService } from "ngx-toastr";
import { Observable, catchError, forkJoin, switchMap, throwError } from "rxjs";
import { HttpException } from "../models/http-exception.model";
import { TranslateService } from "@ngx-translate/core";

@Injectable()
export class HttpExceptionInterceptor implements HttpInterceptor {

  constructor(
    private toastr: ToastrService,
    private trans: TranslateService
  ){}

  errors = [];
  //TODO: mb simply try instant instead of get?
  subs = []

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((err) => {
        if (err instanceof HttpErrorResponse) {
          console.error(err);
          switch (err.status) {
            case 400: {
              this.handle400(err);
              break;
            }
            case 401:
              this.errors.push(err.statusText)
              break;
            case 403:
              this.handle403(err);
              break;
            case 404:
              this.handle404(err);
              break;
            case 500:
              this.handle500();
              break;
            default:
              this.handleDefault(err);
              break;
          }
        }

        if(err instanceof Error) {
          this.errors.push(err.message);
        }

        return forkJoin(this.subs).pipe(switchMap(val => {
          const httpException: HttpException = {
            statusCode: err.status,
            errors: this.errors,
            error: err
          }

          return throwError(() => httpException);
        }))
      })
    );
  }

  handle400(err) {
    if (err.error.errors) {
      //check if err.error.errors is array
      if (Array.isArray(err.error.errors)) {
        let validErrors = err.error.errors;
        for(const element of validErrors)
          this.errors.push(element.ErrorMessage);
        }
      else {
        Object.keys(err.error.errors).forEach(key => {
          this.errors.push(err.error.errors[key][0]);
        });
      }

      return;
    }

    if (err.error) {
      this.errors.push(err.error);
    }
  }

  handle403(err) {
    if(err.error?.ExpiresAt && err.error?.Reason) {
      let banReason$ = this.trans.get('labels.you-banned-until')
      .pipe(switchMap(bannedLabel => {
        return this.trans.get('labels.reason')
        .pipe(switchMap(reasonLabel => {
          let msg = `${bannedLabel} ${new Date(err.error.ExpiresAt).toLocaleString()}, ${reasonLabel}: ${err.error.Reason}`;

          this.errors.push(msg);
          this.toastr.error(msg);
          return reasonLabel;
        }))
      }))

      this.subs.push(banReason$);
    }

    let accessDenied$ = this.trans.get('labels.access-denied')
    .pipe(switchMap(label => {
      this.errors.push(label);
      return label;
    }))

    this.subs.push(accessDenied$);
  }

  handle404(err) {
    if(err.error) {
      this.errors.push(err.error);
    } else {
      let notFound$ = this.trans.get('labels.fail-request')
      .pipe(switchMap(label => {
        this.errors.push(label);
        return label;
      }))

      this.subs.push(notFound$);
    }
  }

  handle500() {
    let internalError$ = this.trans.get('lables.internal-error')
    .pipe(switchMap(label => {
      this.errors.push(label);
      return label;
    }))

    this.subs.push(internalError$);
  }

  handleDefault(err) {
    // connection refused or timeout
    if(err.status == 0) {
      let connectionRefused$ = this.trans.get('labels.connection-refused')
      .pipe(switchMap(label => {
        this.errors.push(label);
        return label;
      }))

      this.subs.push(connectionRefused$);

      return;
    }

    let unknownError$ = this.trans.get('labels.something-went-wrong')
    .pipe(switchMap(label => {
      this.errors.push(label);
      return label;
    }))

    this.subs.push(unknownError$);
  }
}
