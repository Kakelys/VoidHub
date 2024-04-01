import { HashLocationStrategy } from '@angular/common';
import { Injectable } from "@angular/core";
import { HubConnection, HubConnectionBuilder, HubConnectionState, Subject } from "@microsoft/signalr";
import { environment as env } from "src/environments/environment";


@Injectable()
export class SignalrService {

  methods: Map<string, {(hub: HubConnection): void}> = new Map();

  hub: HubConnection;
  constructor() {}

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
