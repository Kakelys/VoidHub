import { Component, OnDestroy } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ReplaySubject, take, takeUntil } from 'rxjs';
import { AuthService } from 'src/app/auth/auth.service';
import { User } from 'src/shared/models/user.model';
import { NgFormExtension } from 'src/shared/ng-form.extension';
import { AccountService } from '../account.service';
import { ToastrService } from 'ngx-toastr';
import { environment as env } from 'src/environments/environment';
import { HttpEvent } from '@angular/common/http';
import { HttpException } from 'src/shared/models/http-exception.model';
import { Router } from '@angular/router';
import { EmailService } from '../email.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent implements OnDestroy {
  errorMessages: string[] = [];

  avatarUrl: string = null;
  urlDynamicParam: string = Date.now().toString();
  fileToUpload: File | null = null;
  fileUploading = false;
  avatarProgress: number = 0;

  user: User;
  private destroy$ = new ReplaySubject<boolean>(1);

  constructor(
    private authService: AuthService,
    private accountService: AccountService,
    private emailService: EmailService,
    private toastr: ToastrService,
    private router: Router,
    private trans: TranslateService
  ) {
    authService.user$
    .pipe(takeUntil(this.destroy$))
    .subscribe(user => {
      if(!user)
      {
        this.router.navigate(['/']);
        return;
      }

      this.user = user
      this.urlDynamicParam = Date.now().toString();
      this.avatarUrl = `${env.resourceURL}/${user.avatarPath}`;
    });
  }

  onProfileSubmit(form: NgForm) {
    this.errorMessages = [];
    let data = form.value;

    if(data.oldPassword && !data.newPassword || !data.oldPassword && data.newPassword) {
      this.errorMessages.push("Both old and new password must be filled");
      return;
    }

    if(form.invalid || data.newPassword && data.newPassword.length < 8) {
      NgFormExtension.markAllAsTouched(form);
      return;
    }

    this.accountService.updAccount(data).subscribe({
      next: (user: User) => {
        this.toastr.success('Account updated successfully');
        this.authService.updateUser(user);
      },
      error: (err:HttpException) => {
        this.errorMessages = err.errors;
      }
    })
  }

  handleFileInput(target: any) {
    this.fileToUpload = target.files[0];
  }

  onAvatarSubmit(form: NgForm) {
    this.errorMessages = [];
    let data = form.value;

    if(form.invalid) {
      NgFormExtension.markAllAsTouched(form);
      return;
    }

    if(!this.fileToUpload) {
      this.errorMessages.push("File is required");
      return;
    }

    if(this.fileToUpload.size > env.maxAvatarSize) {
      this.errorMessages.push(`File size must be less than ${env.maxAvatarSize / 1024} KB`);
      return;
    }

    this.fileUploading = true;

    let formData = new FormData();
    formData.append('img', this.fileToUpload);
    this.accountService.updAvatar(formData, this.user.avatarPath).subscribe({
      next: (event: HttpEvent<User>) => {
        if(event.type == 1) {
          this.avatarProgress = Math.round(100 * event.loaded / event.total);
        }
        if(event.type == 4) {
          if(event.body) {
            this.authService.updateUser(event.body);
          } else {
            this.urlDynamicParam = Date.now().toString();
          }
          this.toastr.success('Avatar updated successfully');
          this.fileToUpload = null;
          form.reset();
        }
      },
      error: (err: HttpException) => {
        this.errorMessages = err.errors;
      },
      complete: () => {
        this.fileUploading = false;
      }
    })
  }

  onResendClick() {
    this.emailService.resendConfirmEmail()
    .subscribe({
      next: _ => {
        this.trans.get('labels.done-check-email')
        .pipe(take(1))
        .subscribe(str => {
          this.toastr.success(str);
        })
      },
      error: (err:HttpException) => {
        this.errorMessages = err.errors;
      }
    })
  }

  ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.complete();
  }
}
