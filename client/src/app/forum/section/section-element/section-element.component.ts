import { TranslateService } from '@ngx-translate/core'
import { ToastrService } from 'ngx-toastr'

import { Component, EventEmitter, Input, Output } from '@angular/core'

import { HttpException } from 'src/shared/models/http-exception.model'
import { User } from 'src/shared/models/user.model'
import { Roles } from 'src/shared/roles.enum'
import { ToastrExtension } from 'src/shared/toastr.extension'

import { SectionFullResponse } from '../../models/section-full-response.model'
import { SectionService } from '../../services/section.service'

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
