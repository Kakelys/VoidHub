import { Component } from '@angular/core';
import { environment } from 'src/environments/environment';
import { RecoverService } from '../recover.service';
import { NgForm } from '@angular/forms';
import { NgFormExtension } from 'src/shared/ng-form.extension';
import { HttpException } from 'src/shared/models/http-exception.model';
import { ToastrService } from 'ngx-toastr';
import { ToastrExtension } from 'src/shared/toastr.extension';

@Component({
  selector: 'app-password-recover-request',
  templateUrl: './password-recover-request.component.html',
  styleUrls: ['./password-recover-request.component.css']
})
export class PasswordRecoverRequestComponent {

  isSended = false;
  limitNames = environment.limitNames;

  constructor(
    private recoverService: RecoverService,
    private toastr: ToastrService) {}

  onSubmit(form: NgForm) {
    if(form.invalid) {
      NgFormExtension.markAllAsTouched(form);
      return;
    }

    this.recoverService.sendEmail(form.value.loginOrEmail)
    .subscribe({
      error: (err: HttpException) => {
        ToastrExtension.handleErrors(this.toastr, err.errors);
      },
      complete: () => {
        this.isSended = true;
      }
    })
  }
}
