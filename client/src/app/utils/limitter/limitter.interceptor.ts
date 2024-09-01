import { finalize } from 'rxjs'

import { HttpHandler, HttpRequest } from '@angular/common/http'
import { Injectable } from '@angular/core'

import { environment as env } from 'src/environments/environment'

import { LimiterService } from './limitter.service'

@Injectable()
export class LimiterInterceptor {
    constructor(private limiter: LimiterService) {}

    intercept(req: HttpRequest<any>, next: HttpHandler) {
        const isSkip = req.headers.get(env.limitNames.skipParam) ?? false
        if (isSkip) {
            const reqMod = req.clone({ headers: req.headers.delete(env.limitNames.skipParam) })
            return next.handle(reqMod)
        }

        const reqName = req.headers.get(env.limitNames.nameParam) ?? this.limiter.defaultName
        const limitParam = req.headers.get(env.limitNames.limitParam) ?? this.limiter.defaultLimit
        if (this.limiter.isOutOfLimit(reqName, this.limiter.defaultLimit + +limitParam)) {
            throw new Error('Too much requests')
        }

        this.limiter.plus(reqName)

        // modify req, removing all params
        const headers = req.headers
        headers.delete(env.limitNames.limitParam)
        headers.delete(env.limitNames.nameParam)
        const reqMod = req.clone({ headers: headers })

        return next.handle(reqMod).pipe(
            finalize(() => {
                this.limiter.minus(reqName)
            })
        )
    }
}
