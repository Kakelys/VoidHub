import { Injectable, OnDestroy } from "@angular/core";
import { HubConnection, HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import { BehaviorSubject, ReplaySubject, takeUntil } from 'rxjs';
import { AuthService } from 'src/app/auth/auth.service';
import { environment as env } from "src/environments/environment";


@Injectable()
export class SignalrService implements OnDestroy {

  connected$ = new BehaviorSubject<boolean>(false);
  hub: HubConnection;

  private destroy$ = new ReplaySubject(1);

  constructor(private auth: AuthService) {
    this.auth.user$.pipe(takeUntil(this.destroy$))
    .subscribe(user => {
      if(user)
        this.start();
      else
        this.stop();
    })
  }

  ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.complete();
  }

  public async start() {
    if(this.hub)
      await this.stop();

    this.hub = new HubConnectionBuilder()
    .withUrl(env.baseAPIUrl + "/v1/signalr", {
      accessTokenFactory: () => localStorage.getItem('access-token'),
      withCredentials: false
    })
    .withAutomaticReconnect()
    .build();

    this.hub.start()
    .then(_ => {
      console.log('signalR: connected');
      this.connected$.next(true);
    })
    .catch(err => {
      console.log('signalR error ', err)
    });
  }

  public async stop() {
    if(this.hub && this.hub.state != HubConnectionState.Disconnected)
      await this.hub.stop();
  }
}
