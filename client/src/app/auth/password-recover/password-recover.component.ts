import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { HttpException } from 'src/shared/models/http-exception.model';
import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';
import { RecoverService } from '../recover.service';
import { NgForm } from '@angular/forms';
import { NgFormExtension } from 'src/shared/ng-form.extension';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { TranslateService } from '@ngx-translate/core';
import { take } from 'rxjs';
import { ToastrExtension } from 'src/shared/toastr.extension';

@Component({
  selector: 'app-password-recover',
  templateUrl: './password-recover.component.html',
  styleUrls: ['./password-recover.component.css']
})
export class PasswordRecoverComponent {

  limitNames = environment.limitNames;

  constructor(
    private recoverService: RecoverService,
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService,
    private trans: TranslateService
  ) {}

  onSubmit(form: NgForm) {
    let data = form.value;
    if(form.invalid || data.password != data.confirmPassword) {
      NgFormExtension.markAllAsTouched(form);
      return;
    }

    this.recoverService.recover(this.route.snapshot.params['token'], data.password)
    .subscribe({
      next: _ => {
        this.trans.get('labels-using-new-password')
        .pipe(take(1))
        .subscribe(str => {
          this.toastr.success(str);
          this.router.navigate(['/login']);
        })
      },
      error: (err: HttpException) => {
        ToastrExtension.handleErrors(this.toastr, err.errors);
      }
    })
  }
}
