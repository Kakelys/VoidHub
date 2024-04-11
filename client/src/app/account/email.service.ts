import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";

@Injectable()
export class EmailService {
  private baseUrl: string = environment.baseAPIUrl + "/v1/email";

  constructor(private http: HttpClient) {}

  confirmEmail(token) {
    const headers = new HttpHeaders().set(environment.limitNames.nameParam, environment.limitNames.confirmEmail);

    return this.http.get(this.baseUrl + "/confirm", {
      params: {
        base64Token: token
      }
    })
  }

  resendConfirmEmail() {
    return this.http.get(this.baseUrl + "/resend-confirm");
  }
}
