import { Component, Input } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { debounceTime, take, takeUntil } from 'rxjs';
import { ConnectionsDataNotification } from 'src/app/notify/models/connections-data-notifications.model';
import { UserNotification } from 'src/app/notify/models/user-notification.model';
import { NotifyService } from 'src/app/notify/notify.service';
import { User } from 'src/shared/models/user.model';
import { SignalrService } from 'src/shared/signalr.service';
import { StatsService } from '../stats.service';
import { OnlineStats } from '../online-stats.model';

@Component({
  selector: 'app-online-stats',
  templateUrl: './online-stats.component.html',
  styleUrls: ['./online-stats.component.css']
})
export class OnlineStatsComponent {

  onlineStats: OnlineStats = {anonimCount: 0, users: []};

  @Input()
  containerClasses = 'bg-base-200 rounded p-3'

  constructor(statsService: StatsService) {
    statsService.onlineStats$
    .pipe(takeUntilDestroyed())
    .subscribe(stats => {
      this.onlineStats = stats;
    })
  }
}
