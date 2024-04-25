import { HttpHandler, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { finalize} from "rxjs";
import { LimitterService } from "./limitter.service";
import { environment as env } from "src/environments/environment";

@Injectable()
export class LimitterInterceptor {

  constructor(private limitter: LimitterService){}

  intercept (
    req: HttpRequest<any>,
    next: HttpHandler
  ) {
    let isSkip = req.headers.get(env.limitNames.skipParam) ?? false;
    if(isSkip)
    {
      const reqMod = req.clone({ headers: req.headers.delete(env.limitNames.skipParam) });
      return next.handle(reqMod);
    }

    let reqName = req.headers.get(env.limitNames.nameParam) ?? this.limitter.defaultName;
    let limitParam = req.headers.get(env.limitNames.limitParam) ?? this.limitter.defaultLimit;
    if(this.limitter.isOutOfLimit(reqName, this.limitter.defaultLimit + +limitParam)) {
      throw new Error('Too much requests')
    }

    this.limitter.plus(reqName);

    // modify req, removing all params
    let headers = req.headers;
    headers.delete(env.limitNames.limitParam);
    headers.delete(env.limitNames.nameParam);
    const reqMod = req.clone({ headers: headers});

    return next.handle(reqMod)
    .pipe(finalize(
      () => {
        this.limitter.minus(reqName);
      },
    ));
  }
}
