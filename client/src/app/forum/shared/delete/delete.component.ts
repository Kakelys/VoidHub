import { TranslateModule, TranslateService } from '@ngx-translate/core'
import { take } from 'rxjs'

import { CommonModule } from '@angular/common'
import { Component, EventEmitter, Input, Output } from '@angular/core'

@Component({
    selector: 'app-delete',
    templateUrl: './delete.component.html',
    styleUrls: ['./delete.component.css'],
    standalone: true,
    imports: [CommonModule, TranslateModule],
})
export class DeleteComponent {
    @Input()
    confirmContent = ''

    @Output()
    confirmed = new EventEmitter()

    @Input()
    height = '30px'

    @Input()
    width = '20px'

    constructor(trans: TranslateService) {
        trans
            .get('lables.are-you-sure')
            .pipe(take(1))
            .subscribe((str) => {
                this.confirmContent = str
            })
    }

    confirmClick() {
        this.confirmed.emit()
    }
}
