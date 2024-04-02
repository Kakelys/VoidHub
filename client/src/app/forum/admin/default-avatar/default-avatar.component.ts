import { Component, Input } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { ToastrService } from 'ngx-toastr';
import { ToastrExtension } from 'src/shared/toastr.extension';
import { HttpException } from 'src/shared/models/http-exception.model';

@Component({
  selector: 'app-default-avatar',
  templateUrl: './default-avatar.component.html',
  styleUrls: ['./default-avatar.component.css']
})
export class DefaultAvatarComponent {

  @Input()
  accountId: string;

  constructor(private accountService: AccountService,
    private toastr: ToastrService) {}

  onDefaultClick() {
    console.log(this.accountId)
    if(!this.accountId)
    {
      this.toastr.error('Account id not provided');
      return;
    }

    this.accountService.defaultAvatar(this.accountId)
      .subscribe({
        next: _ => {
          this.toastr.success("Avatar updated successfully");
        },
        error: (err: HttpException) => {
          ToastrExtension.handleErrors(this.toastr, err.errors)
        }
      })
  }

}
