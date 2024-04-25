import { Injectable, OnDestroy } from "@angular/core";
import { NotifyService } from "../notify.service";
import { ToastrService } from "ngx-toastr";
import { Router } from "@angular/router";
import { NewMessageNotification } from "../models/new-message-notification.model";
import { NewMessageComponent } from "./new-message.component";
import { AuthService } from "../../auth/auth.service";
import { ReplaySubject, Subscription, takeUntil } from "rxjs";

@Injectable()
export class NewMessageListener implements OnDestroy {
  private ignoredId: number;

  private destory$ = new ReplaySubject();

  private sub: Subscription;

  constructor(
    private notifyService: NotifyService,
    private toastr: ToastrService,
    private router: Router,
    private auth: AuthService
  ) {
    this.auth.user$.pipe(takeUntil(this.destory$))
    .subscribe(user => {
      if(user) {
        this.ignoredId = user.id;
        this.start();
      }
      else {
        this.stop()
      }
    })
  }

  ngOnDestroy(): void {
    this.destory$.next(true);
    this.destory$.complete();
  }

  public async start() {
    await this.stop();

    this.sub = this.notifyService.getSubject('newMessage')
    .pipe(takeUntil(this.destory$))
    .subscribe((notify: NewMessageNotification) => {
      this.onNewMessage(notify)
    })
  }

  public async stop() {
    if(!this.sub)
      return;

    this.sub.unsubscribe();
  }

  private onNewMessage(notify: NewMessageNotification) {
    if(this.router.url.indexOf("chats") != -1 || notify.sender.id == this.ignoredId)
      return;

    this.toastr.success(notify.message.content, notify.sender.username, {
      tapToDismiss: true,
      toastComponent: NewMessageComponent,
      toastClass: "",
      positionClass: "toast-bottom-right"
    });
  }
}
