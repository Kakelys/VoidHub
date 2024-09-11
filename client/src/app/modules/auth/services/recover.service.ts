import { HttpClient, HttpHeaders } from '@angular/common/http'
import { Injectable } from '@angular/core'

import { environment } from 'src/environments/environment'

@Injectable()
export class RecoverService {
    baseUrl = environment.baseAPIUrl + '/v1/email'
    constructor(private http: HttpClient) {}

    sendEmail(loginOrEmail: string) {
        const headers = new HttpHeaders().set(
            environment.limitNames.nameParam,
            environment.limitNames.sendRecover
        )

        return this.http.get(this.baseUrl + '/send-recover', {
            headers: headers,
            params: {
                loginOrEmail: loginOrEmail,
            },
        })
    }

    recover(token: string, password: string) {
        const headers = new HttpHeaders().set(
            environment.limitNames.nameParam,
            environment.limitNames.sendRecover
        )

        return this.http.post(
            this.baseUrl + '/recover',
            {
                base64Token: token,
                password: password,
            },
            {
                headers: headers,
            }
        )
    }
}
