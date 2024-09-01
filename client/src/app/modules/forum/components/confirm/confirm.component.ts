import { Component, EventEmitter, Input, Output } from '@angular/core'

@Component({
    selector: 'app-confirm',
    templateUrl: './confirm.component.html',
    styleUrls: ['./confirm.component.css'],
})
export class ConfirmComponent {
    @Input()
    confirmContent = 'Are you sure?'

    @Input()
    confirmContainerClasses = 'btn btn-info min-h-min max-w-fit'

    @Input()
    containerClasses = 'btn btn-ghost min-h-min h-fit w-fit p-1 rounded'

    @Output()
    confirmed = new EventEmitter()

    confirmClick() {
        this.confirmed.emit()
    }
}
