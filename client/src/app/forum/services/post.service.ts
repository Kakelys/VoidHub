import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Offset } from "src/shared/offset.model";
import { environment as env } from "src/environments/environment";
import { Subject, tap } from "rxjs";

@Injectable()
export class PostService {

  private baseURL = env.baseAPIUrl + '/v1/posts';
  public postCreated$ = new Subject<void>();

  constructor(private http: HttpClient) {}

  getComments(ancestorId, offset: Offset) {
    return this.http.get(`${this.baseURL}/${ancestorId}/comments`, {
      headers: {
        'X-Limit-Param': '2'
      },
      params: {...offset}
    })
  }

  getImages(postId: number) {
    return this.http.get(`${this.baseURL}/${postId}/images`)
  }

  createPost(data) {
    return this.http.post(this.baseURL, data)
      .pipe(tap(_ => {this.postCreated$.next()}));
  }

  updatePost(postId, data) {
    return this.http.put(`${this.baseURL}/${postId}`, data);
  }

  deletePost(postId) {
    return this.http.delete(`${this.baseURL}/${postId}`);
  }
}
