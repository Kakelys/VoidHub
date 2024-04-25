import { Component, OnDestroy, OnInit } from '@angular/core';
import { ReplaySubject, takeUntil } from 'rxjs';
import { AuthService } from '../../auth/auth.service';
import { User } from 'src/shared/models/user.model';
import { Roles } from 'src/shared/roles.enum';
import { SectionService } from '../../forum/services/section.service';
import { SectionResponse } from '../../forum/models/section-reponse.model';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.css']
})
export class MenuComponent implements OnInit, OnDestroy {

  user: User;
  roles = Roles;

  sections: SectionResponse[] = [];

  private destory$ = new ReplaySubject<boolean>(1);

  constructor(
  private sectionService: SectionService,
  auth: AuthService) {
    auth.user$
    .pipe(takeUntil(this.destory$))
    .subscribe(user => {
      this.user = user;
    })
  }

  ngOnInit(): void {
    this.sectionService.getShortSections()
    .subscribe({
      next: (data: SectionResponse[]) => {
        this.sections.push(...data);
      }
    });
  }

  ngOnDestroy(): void {
    this.destory$.next(true);
    this.destory$.complete();
  }
}
