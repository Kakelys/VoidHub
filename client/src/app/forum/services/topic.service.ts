import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Offset } from "src/shared/offset.model";
import { environment as env } from "src/environments/environment";
import { Subject, tap } from "rxjs";

@Injectable()
export class TopicService {

  private baseURL = env.baseAPIUrl + '/v1/topics';
  public topicCreated$ = new Subject<void>();

  constructor(
    private http: HttpClient,
  ) {}

  getTopic(topicId, offset: Offset) {
    const headers = new HttpHeaders().set(env.limitNames.nameParam, env.limitNames.topicLoad);

    return this.http.get(`${this.baseURL}/${topicId}`, {
      headers,
      params: {...offset}
    });
  }

  getTopics(offset: Offset, time: Date) {
    return this.http.get(this.baseURL, {
      params: {
        ...offset,
        time: time.toISOString()
      }
    })
  }

  createTopic(topic) {
    return this.http.post(this.baseURL, topic)
    .pipe(tap(_ => {this.topicCreated$.next()}));;
  }

  updateTopic(topicId, data) {
    return this.http.put(this.baseURL + '/' + topicId, data);
  }

  deleteTopic(topicId) {
    return this.http.delete(this.baseURL + '/' + topicId);
  }

  recoverTopic(topicId) {
    return this.http.patch(this.baseURL + '/' + topicId + '/recover', {});
  }
}
