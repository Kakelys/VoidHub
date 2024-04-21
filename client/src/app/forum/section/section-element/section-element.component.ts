import { Component, EventEmitter, Input, Output } from '@angular/core';
import { SectionService } from '../../services/section.service';
import { User } from 'src/shared/models/user.model';
import { Roles } from 'src/shared/roles.enum';
import { SectionResponse } from '../../models/section-reponse.model';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { HttpException } from 'src/shared/models/http-exception.model';
import { ToastrExtension } from 'src/shared/toastr.extension';


@Component({
  selector: 'app-section',
  templateUrl: './section-element.component.html',
  styleUrls: ['./section-element.component.css']
})
export class SectionElementComponent {

  @Output()
  onSectionDeleted = new EventEmitter<number>();

  @Input()
  data: SectionResponse;

  @Input()
  user: User;

  roles = Roles;

  editMode = false;

  constructor(private sectionService: SectionService,
    private toastr: ToastrService,
    private trans: TranslateService
  ) {
  }

  onEdit(data) {
    this.sectionService.updateSection(this.data.section.id, data)
    .subscribe({
      next: (section:any) => {
        this.data.section.title = section.title;
      }
    })
  }

  onDelete() {
    this.sectionService.deleteSection(this.data.section.id)
    .subscribe({
      next: (section:any) => {
        this.onSectionDeleted.emit(this.data.section.id);
      },
      error: (err: HttpException) => {
        ToastrExtension.handleErrors(this.toastr, err.errors);
      }
    })
  }
}
