import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, of, tap } from "rxjs";
import { User } from "../../shared/models/user.model";
import { Register } from "./models/register.model";
import { AuthResponse } from "./models/auth-response.model";
import { Login } from "./models/login.model";
import { environment as env } from "src/environments/environment";
import { jwtDecode } from "jwt-decode";

@Injectable({
  providedIn: 'root'
})
export class AuthService
{
  private user = new BehaviorSubject<User>(null);
  user$ = this.user.asObservable();

  private baseURL:string = env.baseAPIUrl + '/v1/auth/';

  constructor(private http: HttpClient) {}

  public login(login: Login) {
    return this.http.post<AuthResponse>(this.baseURL + 'login', login)
      .pipe(tap(data => this.handleAuth(data)));
  }
  public register(register: Register) {
    return this.http.post<AuthResponse>(this.baseURL + 'register', register)
      .pipe(tap(data => this.handleAuth(data)));
  }

  public logout() {
    localStorage.removeItem('access-token');
    localStorage.removeItem('refresh-token');
    localStorage.removeItem('user');

    this.user.next(null);
  }

  public setUser(user: User) {
    this.user.next(user);
  }

  public updateUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.user.next(user);
  }

  public refreshAndAuth() {
    //refresh and handle auth
    const refreshToken = localStorage.getItem('refresh-token');
    if(!refreshToken)
      return of(null);

    return this.http.get<AuthResponse>(this.baseURL + 'refresh?refreshToken=' + refreshToken, {
      headers: {
        'X-Limit-Skip': 'true'
      }
      })
      .pipe(tap(data => this.handleAuth(data)))
  }

  public setRefreshTimeout()
  {
    const access = localStorage.getItem('access-token');
    const refresh = localStorage.getItem('refresh-token');

    if(!access && !refresh)
      return;

    if(!access && refresh) {
      this.refreshAndAuth().subscribe();
      return;
    }

    const jwtPayload = jwtDecode(access);

    // milliseconds until force refresh token
    let ms = jwtPayload.exp * 1000 - new Date().valueOf();
    //console.log('set refresh timer ', ms / 1000 / 60, ' min')
    setTimeout(_ => {
      const tmpAccess = localStorage.getItem('access-token');
      if(!tmpAccess)
        return;

      const newJwtPayload = jwtDecode(tmpAccess);
      // if already updated
      if(newJwtPayload.exp * 1000 - new Date().valueOf() > 0)
        return;

      this.refreshAndAuth().subscribe();
    }, ms)
  }

  private handleAuth(authResponse:AuthResponse) {
    console.log('handle auth')
    if(!authResponse || !authResponse.user || !authResponse.tokens)
      return;

    localStorage.setItem('access-token', authResponse.tokens.accessToken);
    localStorage.setItem('refresh-token', authResponse.tokens.refreshToken);

    localStorage.setItem('user', JSON.stringify(authResponse.user));

    this.user.next(authResponse.user);

    // set timer to next force refresh
    this.setRefreshTimeout();
  }
}
