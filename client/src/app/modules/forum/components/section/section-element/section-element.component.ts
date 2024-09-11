import { TranslateService } from '@ngx-translate/core'
import { ToastrService } from 'ngx-toastr'

import { Component, EventEmitter, Input, Output } from '@angular/core'

import { Roles } from 'src/shared/roles.enum'

import { HttpException } from 'src/app/common/models'
import { ToastrExtension } from 'src/app/common/utils'
import { User } from 'src/app/modules/auth'

import { SectionService } from '../../../services'
import { SectionFullResponse } from '../../../types'

@Component({
    selector: 'app-section',
    templateUrl: './section-element.component.html',
    styleUrls: ['./section-element.component.css'],
})
export class SectionElementComponent {
    @Output()
    sectionDeleted = new EventEmitter<number>()

    @Input()
    data: SectionFullResponse

    @Input()
    user: User

    roles = Roles

    editMode = false

    constructor(
        private sectionService: SectionService,
        private toastr: ToastrService,
        private trans: TranslateService
    ) {}

    onEdit(data) {
        this.sectionService.updateSection(this.data.section.id, data).subscribe({
            next: (section: any) => {
                this.data.section.title = section.title
            },
        })
    }

    onDelete() {
        this.sectionService.deleteSection(this.data.section.id).subscribe({
            next: (section: any) => {
                this.sectionDeleted.emit(this.data.section.id)
            },
            error: (err: HttpException) => {
                ToastrExtension.handleErrors(this.toastr, err.errors)
            },
        })
    }
}
