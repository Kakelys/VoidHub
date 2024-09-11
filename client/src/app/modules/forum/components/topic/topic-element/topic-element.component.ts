import { TranslateModule } from '@ngx-translate/core'

import { CommonModule } from '@angular/common'
import { Component, Input } from '@angular/core'
import { RouterModule } from '@angular/router'

import { TimezonePipe } from 'src/shared/timemezone.pipe'
import { TruncatePipe } from 'src/shared/truncate.pipe'

import { environment } from 'src/environments/environment'

import { ClosedIconComponent } from '../../closed-icon/closed-icon.component'
import { PinnedIconComponent } from '../../pinned-icon/pinned-icon.component'

@Component({
    selector: 'app-topic-element',
    templateUrl: './topic-element.component.html',
    styleUrls: ['./topic-element.component.css'],
    standalone: true,
    imports: [
        CommonModule,
        PinnedIconComponent,
        ClosedIconComponent,
        RouterModule,
        TruncatePipe,
        TimezonePipe,
        TranslateModule,
    ],
})
export class TopicElementComponent {
    @Input()
    topic

    resourceUrl = environment.resourceURL
}
