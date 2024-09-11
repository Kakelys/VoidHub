import { HttpClient } from '@angular/common/http'
import { Injectable } from '@angular/core'

import { environment as env } from 'src/environments/environment'

@Injectable()
export class UploadService {
    private baseURL = env.baseAPIUrl + '/v1/uploads/images'

    constructor(private http: HttpClient) {}

    upload(data: FormData) {
        return this.http.post(this.baseURL, data)
    }

    delete(id: number) {
        return this.http.delete(`${this.baseURL}/${id}`)
    }

    deleteMany(ids: number[]) {
        return this.http.delete(`${this.baseURL}`, {
            params: {
                ids,
            },
        })
    }
}
