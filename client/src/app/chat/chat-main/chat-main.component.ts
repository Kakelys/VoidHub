import { ChatResponse } from './../models/chat-response.model';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { ChatService } from '../services/chat.service';
import { ReplaySubject, takeUntil } from 'rxjs';
import { HttpException } from 'src/shared/models/http-exception.model';
import { ToastrService } from 'ngx-toastr';
import { ToastrExtension } from 'src/shared/toastr.extension';
import { Offset } from 'src/shared/offset.model';
import { environment as env } from 'src/environments/environment';
import { NotifyService } from 'src/app/notify/notify.service';
import { NewMessageNotification } from 'src/app/notify/new-message-notification.model';
import { AuthService } from 'src/app/auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-chat-main',
  templateUrl: './chat-main.component.html',
  styleUrls: ['./chat-main.component.css']
})
export class ChatMainComponent implements OnDestroy, OnInit {

  public chatId : number = null;
  public chats: ChatResponse[] =  [];
  loadTime = new Date();
  canLoadMore = true;
  chatLimit = 5;

  resourceUrl = env.resourceURL;
  limitNames = env.limitNames;

  public destroy$ = new ReplaySubject<boolean>();

  constructor(
    private chat: ChatService,
    private cdr: ChangeDetectorRef,
    private toastr: ToastrService,
    private notifyService: NotifyService,
    private auth: AuthService,
    private router: Router
  ) {
  }

  ngOnInit(): void {
    this.chat.currentChat$.subscribe(chatId => {
      this.chatId = chatId;
      this.cdr.detectChanges();
    })

    this.notifyService.getSubject('newMessage')
    .pipe(takeUntil(this.destroy$))
    .subscribe((notify: NewMessageNotification) => {
      this.onNewMessage(notify);
    })

    this.auth.user$
    .pipe(takeUntil(this.destroy$))
    .subscribe(user => {
      if(!user)
        this.router.navigate(['/']);
    })

    this.loadNextChats();
  }

  loadNextChats() {
    if(!this.canLoadMore)
      return;

    const additionalOffset = this.chats.filter(c => c.lastMessage.createdAt > this.loadTime).length;
    const offset = new Offset(this.chats.length + additionalOffset, this.chatLimit);

    this.chat.getChats(offset, this.loadTime).subscribe({
      next: (chats: ChatResponse[]) => {
        if(!chats || chats.length < this.chatLimit) {
          this.canLoadMore = false;
        }

        this.chats.push(...chats);
      },
      error: (err: HttpException) =>
        ToastrExtension.handleErrors(this.toastr, err.errors)
    })
  }

  onNewMessage(notify: NewMessageNotification) {
    this.chats = this.chats.filter(c => c.chat.id != notify.chat.id);
    this.chats.unshift({
      chat: notify.chat,
      lastMessage: notify.message,
      sender: notify.sender,
      anotherUser: notify.anotherUser
    })
  }

  ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.complete();
  }
}
