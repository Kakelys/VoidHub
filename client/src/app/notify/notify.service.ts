import { Injectable } from "@angular/core";
import { HubConnection } from "@microsoft/signalr";
import { ToastrService } from "ngx-toastr";
import { SignalrService } from "src/shared/signalr.service";
import { NotificationBase } from "./notifcation-base.model";
@Injectable()
export class NotifyService {

  private subscribes: Map<string, {timestamp: Date, callback: (notifyData: NotificationBase) => void}[]> = new Map();

  constructor(private signalR: SignalrService, private toastr: ToastrService) {
    this.signalR.addToFactory('notify', this.startRecievingNotifies.bind(this));
  }

  private startRecievingNotifies(hub: HubConnection) {
    hub.on('notify', (ntf: NotificationBase) => {
      if(!this.subscribes.has(ntf.type)) {
        this.toastr.info("Notify)");
        return;
      }

      this.subscribes.get(ntf.type).forEach(n => {
        n.callback(ntf);
      })
    })
  }

  subscribe(type: string, callbackObj: {timestamp: Date, callback: (notifyData: NotificationBase) => void}) {
    if(!this.subscribes.has(type))
      this.subscribes.set(type, new Array());

    this.subscribes.get(type).push(callbackObj);
  }

  unsubscribe(type: string, timestamp: Date) {
    if(!this.subscribes.has(type))
      return;

    var idx = this.subscribes.get(type).findIndex(el => el.timestamp == timestamp);
    if(idx == -1)
      return;

    this.subscribes.get(type).splice(idx, 1);
  }
}
