import { Component, OnDestroy, OnInit } from '@angular/core';
import { AccountService } from '../account.service';
import { AuthService } from 'src/app/auth/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ReplaySubject, takeUntil } from 'rxjs';
import { User } from 'src/shared/models/user.model';
import { Roles } from 'src/shared/roles.enum';
import { AdminService } from 'src/app/forum/admin/services/admin.service';
import { environment } from 'src/environments/environment';
import { ChatService } from 'src/app/chat/services/chat.service';
import { HttpExceptionInterceptor } from 'src/shared/error/http-exception.interceptor';
import { HttpException } from 'src/shared/models/http-exception.model';
import { ToastrExtension } from 'src/shared/toastr.extension';
import { ToastrService } from 'ngx-toastr';
import { Chat } from 'src/app/chat/models/chat-model';
import { NgForm } from '@angular/forms';
import { NgFormExtension } from 'src/shared/ng-form.extension';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnDestroy {
  resourceUrl = environment.resourceURL;

  user: User = null;
  profile: any = null;
  userId = null;
  roles = Roles;
  pChat: Chat = null;

  private destroy$ = new ReplaySubject<boolean>(1);

  constructor(
    private accountService: AccountService,
    private route: ActivatedRoute,
    private adminService: AdminService,
    private chatService: ChatService,
    private toastr: ToastrService,
    router: Router,
    authService: AuthService
  ) {

    authService.user$
      .pipe(takeUntil(this.destroy$))
      .subscribe(user => {
        this.user = user;
      });

    adminService.cancelClicked
      .pipe(takeUntil(this.destroy$))
      .subscribe(
        _ => {
          router.navigate(['../'], { relativeTo: this.route });
        }
      )

    this.handleIdChange(this.route.snapshot.params['id']);

    route.params.subscribe(async params => {
      this.handleIdChange(params['id']);
    });

  }

  async handleIdChange(newId: number) {
    if(this.userId == newId)
      return;

    this.userId = newId;

    this.accountService.getAccount(this.userId)
      .subscribe({
        next: (user: any) => {
          this.adminService.user = user;
          this.profile = user;

          if(this.user)
            this.updateChatBetween();
        },
        error: (err: HttpException) => {
          ToastrExtension.handleErrors(this.toastr, err.errors);
        }
      });
  };

  updateChatBetween() {
    this.chatService.getChatBetween(this.profile.id)
      .subscribe({
        next: (chat: Chat | null) => {
          this.pChat = chat;
        },
        error: (err: HttpException) => {
          ToastrExtension.handleErrors(this.toastr, err.errors);
        }
      })
  }

  onStartChat(form: NgForm) {
    if(form.invalid) {
      NgFormExtension.markAllAsTouched(form);
      return;
    }

    this.chatService.createPersonal(form.value.content, this.profile.id)
    .subscribe({
      next: (chat: Chat) => {
        //navigate to chat?
      },
      error: (err: HttpException) => {
        ToastrExtension.handleErrors(this.toastr, err.errors);
      }
    })
  }

  ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.complete();
    this.adminService.user = null;
  }
}
