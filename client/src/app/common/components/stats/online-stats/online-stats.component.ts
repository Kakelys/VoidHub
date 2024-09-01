import { Component, Input } from '@angular/core'
import { takeUntilDestroyed } from '@angular/core/rxjs-interop'
import { OnlineStats } from '../online-stats.model'
import { StatsService } from '../stats.service'

@Component({
    selector: 'app-online-stats',
    templateUrl: './online-stats.component.html',
    styleUrls: ['./online-stats.component.css'],
})
export class OnlineStatsComponent {
    onlineStats: OnlineStats = { anonimCount: 0, users: [] }

    @Input()
    containerClasses = 'bg-base-200 rounded p-3'

    constructor(statsService: StatsService) {
        statsService.onlineStats$.pipe(takeUntilDestroyed()).subscribe((stats) => {
            this.onlineStats = stats
        })
    }
}
