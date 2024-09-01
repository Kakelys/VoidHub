import { Subject } from 'rxjs'

import { HttpClient, HttpHeaders } from '@angular/common/http'
import { Injectable } from '@angular/core'

import { Offset } from 'src/shared'

import { environment as env } from 'src/environments/environment'

@Injectable()
export class ChatService {
    private baseURL: string = env.baseAPIUrl + '/v1/chats'
    public currentChat$ = new Subject<number | null>()

    constructor(private http: HttpClient) {}

    public getChats(offset: Offset, time: Date) {
        const headers = new HttpHeaders().set(env.limitNames.nameParam, env.limitNames.chatLoads)

        return this.http.get(`${this.baseURL}`, {
            headers: headers,
            params: {
                ...offset,
                time: time.toISOString(),
            },
        })
    }

    public getChat(chatId: number) {
        const headers = new HttpHeaders().set(env.limitNames.nameParam, env.limitNames.chatInfoLoad)

        return this.http.get(`${this.baseURL}/${chatId}`, {
            headers: headers,
        })
    }

    public getChatBetween(accountId: number) {
        return this.http.get(`${this.baseURL}/between`, {
            params: {
                targetId: accountId,
            },
        })
    }

    public createPersonal(content: string, accountId: number) {
        return this.http.post(`${this.baseURL}/personal`, {
            content: content,
            targetId: accountId,
        })
    }

    public getMessages(chatId: number, offset: Offset, time: Date) {
        const headers = new HttpHeaders().set(env.limitNames.nameParam, env.limitNames.chatLoadMsgs)

        return this.http.get(`${this.baseURL}/${chatId}/messages`, {
            headers: headers,
            params: {
                ...offset,
                time: time.toISOString(),
            },
        })
    }

    public sendMessage(chatId: number, message: string) {
        return this.http.post(`${this.baseURL}/${chatId}/messages`, {
            content: message,
        })
    }
}
