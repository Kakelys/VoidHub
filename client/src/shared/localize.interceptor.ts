import { HttpHandler, HttpRequest } from '@angular/common/http'
import { Injectable } from '@angular/core'

@Injectable()
export class LocalizeInterceptor {
    intercept(req: HttpRequest<any>, next: HttpHandler) {
        const code = localStorage.getItem('locale')
        if (!code) return next.handle(req)

        const modifiedReq = req.clone({
            headers: req.headers.append('Accept-Language', code),
        })

        return next.handle(modifiedReq)
    }
}
