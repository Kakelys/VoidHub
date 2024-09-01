import { Offset } from './../../shared/offset.model'
import { HttpClient, HttpHeaders } from '@angular/common/http'
import { Injectable } from '@angular/core'
import { environment as env } from 'src/environments/environment'

@Injectable()
export class AccountService {
    private baseURL = env.baseAPIUrl + '/v1/accounts'

    constructor(private client: HttpClient) {}

    getAccount(id: number) {
        return this.client.get(this.baseURL + '/' + id)
    }

    updAccount(data: any) {
        return this.client.patch(this.baseURL, data)
    }

    updAvatar(data: FormData, currentAvatarPath: string) {
        return this.client.patch(`${this.baseURL}/avatar`, data, {
            params: {
                currentPath: currentAvatarPath,
            },
            reportProgress: true,
            responseType: 'json',
            observe: 'events',
        })
    }

    getPosts(accountId: number, belowTime: Date, offset: Offset) {
        const headers = new HttpHeaders().set(
            env.limitNames.nameParam,
            env.limitNames.loadProfilePosts
        )

        return this.client.get(`${this.baseURL}/${accountId}/posts`, {
            headers: headers,
            params: {
                ...offset,
                belowTime: belowTime.toISOString(),
            },
        })
    }

    getTopics(accountId: number, belowTime: Date, offset: Offset) {
        const headers = new HttpHeaders().set(
            env.limitNames.nameParam,
            env.limitNames.loadProfileTopics
        )

        return this.client.get(`${this.baseURL}/${accountId}/topics`, {
            headers: headers,
            params: {
                ...offset,
                belowTime: belowTime.toISOString(),
            },
        })
    }
}
