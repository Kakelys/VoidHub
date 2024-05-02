import { Component, Input } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { debounceTime, take, takeUntil } from 'rxjs';
import { ConnectionsDataNotification } from 'src/app/notify/models/connections-data-notifications.model';
import { UserNotification } from 'src/app/notify/models/user-notification.model';
import { NotifyService } from 'src/app/notify/notify.service';
import { User } from 'src/shared/models/user.model';
import { SignalrService } from 'src/shared/signalr.service';

@Component({
  selector: 'app-online-stats',
  templateUrl: './online-stats.component.html',
  styleUrls: ['./online-stats.component.css']
})
export class OnlineStatsComponent {

  anonimCount: number = 0;
  users: User[] = new Array();

  @Input()
  containerClasses = 'bg-base-200 rounded p-3'

  constructor(
    notifyService: NotifyService,
    signalR: SignalrService) {
    notifyService.getSubject("connectionsData")
    .pipe(takeUntilDestroyed())
    .subscribe((notif: ConnectionsDataNotification) => this.handleConnectionsData(notif))

    let interval = null;

    signalR.connected$
    .pipe(takeUntilDestroyed(), debounceTime(1000))
    .subscribe((val) => {
      if(val) {
        signalR.hub.invoke("GetConnectionsData");
        // update list once per 5 mins
        interval = setInterval(() => {
          signalR.hub.invoke("GetConnectionsData");
        }, 1000 * 60 * 5)
      }
      else if(interval) {
        clearInterval(interval);
      }
    })
  }

  handleConnectionsData(notif: ConnectionsDataNotification) {
    this.users = notif.users;
    this.anonimCount = notif.totalCount - notif.users.length;
  }
}
