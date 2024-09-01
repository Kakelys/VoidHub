import { Component, Input } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { ToastrService } from 'ngx-toastr';
import { ToastrExtension } from 'src/shared/toastr.extension';
import { HttpException } from 'src/shared/models/http-exception.model';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-default-avatar',
  templateUrl: './default-avatar.component.html',
  styleUrls: ['./default-avatar.component.css']
})
export class DefaultAvatarComponent {

  @Input()
  accountId: string;

  constructor(
    private accountService: AccountService,
    private toastr: ToastrService,
    private trans: TranslateService) {}

  onDefaultClick() {
    if(!this.accountId)
    {
      this.trans.get('labels.account-id-not-provided')
      .subscribe(str => {
        this.toastr.error(str);
      });

      return;
    }

    this.accountService.defaultAvatar(this.accountId)
      .subscribe({
        next: _ => {
          this.trans.get("labels.avatar-updated-successfully")
          .subscribe(str => {
            this.toastr.success(str);
          })
        },
        error: (err: HttpException) => {
          ToastrExtension.handleErrors(this.toastr, err.errors)
        }
      })
  }

}
