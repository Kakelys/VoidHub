import { Injectable, OnDestroy } from "@angular/core";
import { HubConnection } from "@microsoft/signalr";
import { ToastrService } from "ngx-toastr";
import { SignalrService } from "src/shared/signalr.service";
import { NotificationBase } from "./notifcation-base.model";
import { ReplaySubject, Subject, takeUntil } from "rxjs";

@Injectable()
export class NotifyService implements OnDestroy {

  private bindings: Map<string, Subject<NotificationBase>> = new Map();
  private destroy$ = new ReplaySubject<boolean>();

  constructor(private signalR: SignalrService, private toastr: ToastrService) {
    this.signalR.connected$
    .pipe(takeUntil(this.destroy$))
    .subscribe(val => {
      if(val)
        this.startRecievingNotifies(this.signalR.hub);
    })
  }

  ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.complete();
  }

  private startRecievingNotifies(hub: HubConnection) {
    hub.on('notify', (ntf: NotificationBase) => {
      if(!this.bindings.has(ntf.type)) {
        console.log('notify')
        return;
      }

      this.bindings.get(ntf.type).next(ntf);
    })
  }

  getSubject(name: string) : Subject<NotificationBase> {
    if(!this.bindings.has(name))
      this.bindings.set(name, new Subject<NotificationBase>())

    return this.bindings.get(name);
  }
}
