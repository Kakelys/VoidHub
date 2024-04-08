import { HttpClient } from '@angular/common/http';
import { Injectable } from "@angular/core";
import { Roles } from 'src/shared/roles.enum';
import { environment as env } from 'src/environments/environment';

@Injectable()
export class AccountService {

  private baseURL = env.baseAPIUrl + '/v1/accounts';

  constructor(private http: HttpClient) {}

  updateRole(username: string, role: Roles) {
    return this.http.patch(`${this.baseURL}/${username}/role`, {
      role: role
    });
  }

  updateUsername(username: string, data) {
    return this.http.patch(`${this.baseURL}/${username}/rename`, data);
  }

  defaultAvatar(username: string) {
    return this.http.patch(`${this.baseURL}/${username}/avatar-default`, null);
  }
}
