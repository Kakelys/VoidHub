import { HttpClient } from '@angular/common/http';
import { Injectable } from "@angular/core";
import { environment as env } from 'src/environments/environment';

@Injectable()
export class NameService
{
  private baseUrl = env.baseAPIUrl +  '/v1/names';

  constructor(private http: HttpClient) {}

  getSections() {
    return this.http.get(`${this.baseUrl}/section`);
  }

  getForums() {
    return this.http.get(`${this.baseUrl}/forum`);
  }
}
