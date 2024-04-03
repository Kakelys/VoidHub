import { Component, Input, OnChanges, OnDestroy, OnInit } from '@angular/core';
import { BreadcrumbService } from '../services/breadcrumb.service';
import { ReplaySubject } from 'rxjs';
import { Link } from '../models/link.model';

@Component({
  selector: 'app-breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.css']
})
export class BreadcrumbComponent implements OnInit, OnDestroy {

  @Input()
  from: string;

  _id: number;
  @Input() set id(val) {
    this._id = val;
    this.bcService.getFrom(this.from, this.id)
    .subscribe({
      next: (links: Link[]) => {
        console.log(links);
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

  private destroy$ = new ReplaySubject<boolean>(1);

  constructor(private bcService: BreadcrumbService) {

  }

  ngOnInit(): void {

  }

  ngOnDestroy(): void {

  }
}
