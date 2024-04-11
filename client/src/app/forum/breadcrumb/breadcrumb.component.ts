import { Component, Input} from '@angular/core';
import { BreadcrumbService } from '../services/breadcrumb.service';
import { Link } from '../models/link.model';

@Component({
  selector: 'app-breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.css']
})
export class BreadcrumbComponent {

  @Input()
  from: string;

  _id: number;
  @Input() set id(val) {
    this._id = val;
    this.bcService.getFrom(this.from, this.id)
    .subscribe({
      next: (links: Link[]) => {
        if(links)
          this.links = links;
      },
      error: err => {
        console.error(err);
      }
    })
  }
  get id() {
    return this._id;
  }

  links: Link[] = [
    {type: 'section', data: 'name'},
    {type: 'forum', data: 'name2'},
    {type: 'topic', data: 'name3'},
  ]


  constructor(private bcService: BreadcrumbService) {}
}
