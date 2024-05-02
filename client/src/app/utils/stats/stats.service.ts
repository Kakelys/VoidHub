import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";



@Injectable()
export class StatsService {

  baseUrl = environment.baseAPIUrl + '/v1/stats/';

  constructor(private http: HttpClient) {}

  getGeneral() {
    return this.http.get(this.baseUrl + "general");
  }

  getOnline() {
    return this.http.get(this.baseUrl + "online");
  }
}
