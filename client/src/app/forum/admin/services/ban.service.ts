import { HttpClient } from '@angular/common/http';
import { Injectable } from "@angular/core";
import { environment as env } from 'src/environments/environment';

@Injectable()
export class BanService {

  baseUrl = env.baseAPIUrl + '/v1/bans';

  constructor(private http: HttpClient) {}

  public banUser(data) {
    return this.http.post(this.baseUrl, data);
  }

  public unbanUser(username: string) {
    return this.http.delete(`${this.baseUrl}?username=${username}`);
  }
}
