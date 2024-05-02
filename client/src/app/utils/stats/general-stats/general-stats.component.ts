import { Subject } from 'rxjs';
import { Component, Input } from '@angular/core';
import { StatsService } from '../stats.service';
import { GeneralStats } from './general-stats.model';

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

    statsService.getGeneral().subscribe({
      next: (data: GeneralStats) => {
        this.stats = data;
      },
      error: err => {
        console.error(err);
      }
    })
  }


}
