import { HttpClient, HttpHeaders } from '@angular/common/http'
import { Injectable } from '@angular/core'

import { Page } from 'src/shared/page.model'

import { environment as env } from 'src/environments/environment'

@Injectable()
export class ForumService {
    private baseUrl = env.baseAPIUrl + '/v1/forums'

    constructor(private http: HttpClient) {}

    getForum(forumId, params) {
        const headers = new HttpHeaders().set(env.limitNames.nameParam, env.limitNames.forumLoad)

        return this.http.get(`${this.baseUrl}/${forumId}`, {
            headers: headers,
            params: {
                ...params,
            },
        })
    }

    getForumTopics(forumId, page: Page, params) {
        const headers = new HttpHeaders().set(env.limitNames.nameParam, env.limitNames.topicsLoad)

        return this.http.get(`${this.baseUrl}/${forumId}/topics`, {
            headers: headers,
            params: {
                ...page,
                ...params,
            },
        })
    }

    createForum(data) {
        return this.http.post(this.baseUrl, data)
    }

    updateForum(forumId, data) {
        return this.http.put(`${this.baseUrl}/${forumId}`, data)
    }

    deleteForum(forumId) {
        return this.http.delete(`${this.baseUrl}/${forumId}`)
    }
}
