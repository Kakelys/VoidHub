import { Injectable, OnDestroy } from "@angular/core";
import { NotifyService } from "./notify.service";
import { ToastrService } from "ngx-toastr";
import { Router } from "@angular/router";
import { NewMessageNotification } from "./new-message-notification.model";
import { NewMessageComponent } from "./new-message/new-message.component";
import { AuthService } from "../auth/auth.service";
import { ReplaySubject, takeUntil } from "rxjs";

@Injectable()
export class NewMessageListener implements OnDestroy {
  private timestamp: Date;
  private ignoredId: number;

  private destory$ = new ReplaySubject();

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

    this.timestamp = new Date();

    this.notifyService
    .subscribe('newMessage', {
      timestamp: this.timestamp,
      callback: this.onNewMessage.bind(this)
    });
  }

  public async stop() {
    if(!this.timestamp)
      return;

    this.notifyService.unsubscribe('newMessage', this.timestamp);
    this.timestamp = null;
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
