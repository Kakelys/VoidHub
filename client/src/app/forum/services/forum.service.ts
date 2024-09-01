import { Observable, of } from 'rxjs'

import { HttpClient, HttpHeaders } from '@angular/common/http'
import { Injectable } from '@angular/core'

import { Page } from 'src/shared/page.model'

import { environment as env } from 'src/environments/environment'

import { ForumResponse } from '../models/forum-response.model'

@Injectable()
export class ForumService {
    private baseUrl = env.baseAPIUrl + '/v1/forums'

    constructor(private http: HttpClient) {}

    getForum(forumId: number, loadOnlyDeleted: boolean): Observable<ForumResponse> {
        if (!forumId) {
            return of()
        }

        const headers = new HttpHeaders().set(env.limitNames.nameParam, env.limitNames.forumLoad)

        return this.http.get<ForumResponse>(`${this.baseUrl}/${forumId}`, {
            headers: headers,
            params: loadOnlyDeleted ? { onlyDeleted: true } : {},
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
