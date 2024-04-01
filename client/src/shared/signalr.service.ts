import { HashLocationStrategy } from '@angular/common';
import { Injectable, OnDestroy } from "@angular/core";
import { HubConnection, HubConnectionBuilder, HubConnectionState, Subject } from "@microsoft/signalr";
import { ReplaySubject, takeUntil } from 'rxjs';
import { AuthService } from 'src/app/auth/auth.service';
import { environment as env } from "src/environments/environment";


@Injectable()
export class SignalrService implements OnDestroy {

  methods: Map<string, {(hub: HubConnection): void}> = new Map();
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
      this.methods.forEach(m => {
        m(this.hub);
      })
    })
    .catch(err => {
      console.log('signalR error ', err)
    });
  }

  public async stop() {
    if(this.hub && this.hub.state != HubConnectionState.Disconnected)
      await this.hub.stop();
  }

  public addToFactory(name: string, method: (hub: HubConnection) => void) {
    if(this.methods.has(name)){
      console.error("Truing to add existing method to signalR factory: ", name)
      return;
    }

    this.methods.set(name, method);

    if(this.hub && this.hub.state == HubConnectionState.Connected)
      method(this.hub);
  }

  public removeFromFactory(name: string) {
    if(this.methods.has(name))
      this.methods.delete(name);
  }
}
