import { Subject } from 'rxjs';
import { Component, Input } from '@angular/core';
import { StatsService } from '../stats.service';
import { GeneralStats } from '../general-stats.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-general-stats',
  templateUrl: './general-stats.component.html',
  styleUrls: ['./general-stats.component.css']
})
export class GeneralStatsComponent {

  stats: GeneralStats = null;

  @Input()
  containerClasses = 'bg-base-200 rounded p-3'

  constructor(statsService: StatsService) {

    statsService.generalStats$.pipe(takeUntilDestroyed())
    .subscribe(stats => {
      this.stats = stats
    })
  }
}
