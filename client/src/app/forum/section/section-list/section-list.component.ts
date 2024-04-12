import { Component, OnDestroy } from '@angular/core';
import { ReplaySubject, takeUntil } from 'rxjs';
import { AuthService } from 'src/app/auth/auth.service';
import { User } from 'src/shared/models/user.model';
import { SectionService } from '../../services/section.service';
import { Roles } from 'src/shared/roles.enum';
import { ToastrService } from 'ngx-toastr';
import { ToastrExtension } from 'src/shared/toastr.extension';
import { HttpException } from 'src/shared/models/http-exception.model';
import { SectionResponse } from '../../models/section-reponse.model';

@Component({
  selector: 'app-section-list',
  templateUrl: './section-list.component.html',
  styleUrls: ['./section-list.component.css']
})
export class SectionListComponent implements OnDestroy {

  sections: SectionResponse[] = [];
  roles = Roles;

  private destroy$ = new ReplaySubject<boolean>(1);
  user: User = null;

  constructor(
    private authService: AuthService,
    private sectionService: SectionService,
    private toastr: ToastrService
    )
  {
    authService.user$.pipe(takeUntil(this.destroy$))
      .subscribe(user => {this.user = user; });

    sectionService.getSections()
    .subscribe({
      next: (sections: SectionResponse[]) => {
        this.sections = sections;
      },
      error: (err:HttpException) => {
        ToastrExtension.handleErrors(this.toastr, err.errors)
      }
    })
  }

  ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.complete();
  }
}
