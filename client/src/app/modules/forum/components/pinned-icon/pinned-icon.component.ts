import { Component, Input } from '@angular/core'

@Component({
    selector: 'app-pinned-icon',
    templateUrl: './pinned-icon.component.html',
    styleUrls: ['./pinned-icon.component.css'],
    standalone: true,
})
export class PinnedIconComponent {
    @Input()
    height = '20px'

    @Input()
    width = '20px'
}
