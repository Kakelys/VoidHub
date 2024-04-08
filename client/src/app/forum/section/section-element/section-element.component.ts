import { Component, Input } from '@angular/core';
import { SectionService } from '../../services/section.service';
import { User } from 'src/shared/models/user.model';
import { Roles } from 'src/shared/roles.enum';
import { SectionResponse } from '../../models/section-reponse.model';


@Component({
  selector: 'app-section',
  templateUrl: './section-element.component.html',
  styleUrls: ['./section-element.component.css']
})
export class SectionElementComponent {

  @Input()
  data: SectionResponse;

  @Input()
  user: User;

  roles = Roles;

  editMode = false;

  constructor(private sectionService: SectionService) {
  }

  onEdit(data) {
    this.sectionService.updateSection(this.data.section.id, data)
    .subscribe({
      next: (section:any) => {
        this.data.section.title = section.title;
      }
    })
  }
}
