import { HttpHandler, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";



@Injectable()
export class LocalizeInterceptor {
  constructor() {}

  intercept (
    req: HttpRequest<any>,
    next: HttpHandler
  ) {

    let code = localStorage.getItem('locale');
    if(!code)
      return next.handle(req);

    let modifiedReq = req.clone({
      headers: req.headers.append('Accept-Language', code)
    });

    return next.handle(modifiedReq);
  }
}
