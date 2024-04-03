import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { of } from "rxjs";
import { environment } from "src/environments/environment";

@Injectable()
export class BreadcrumbService {

  private baseURL = environment.baseAPIUrl;
  constructor(private http: HttpClient) {
  }

  getFrom(from: string, id: number) {
    switch(from) {
      case 'forum':
        return this.http.get(this.baseURL + "/v1/breadcrumbs/from-forum", {
          params: {
            forumId: id
          }
        });
      case 'topic':
        return this.http.get(this.baseURL + "/v1/breadcrumbs/from-topic", {
          params: {
            topicId: id
          }
        });
      default:
        return of(null);
    }
  }

}
