import { SearchSort } from './../search-sort.enum';
import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/auth/auth.service';
import { User } from '../../../../shared/models/user.model';
import { Roles } from 'src/shared/roles.enum';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { SearchParams } from '../../models/search-params.model';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-search-bar',
  templateUrl: './search-bar.component.html',
  styleUrls: ['./search-bar.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslateModule
  ]
})
export class SearchBarComponent  {

  @Input()
  searchQuery = '';

  @Input()
  searchParams: SearchParams;

  @Input()
  enableParams = false;

  @Output()
  onForceSearch = new EventEmitter();

  roles = Roles;
  user: User;
  sortTypes = SearchSort;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    auth: AuthService
  ) {
    auth.user$
    .pipe(takeUntilDestroyed())
    .subscribe((user: User) => {
      this.user = user;
    })
  }

  onSubmit(form: NgForm) {
    this.router.navigate(["/forum/search"],
    {
      queryParams: {
        query: form.value.search,
        ...this.searchParams,
        ...form.value,
        v: new Date().getMilliseconds(),
      },
      queryParamsHandling: 'merge'
    });
  }
}

