import { ChatResponse } from './../models/chat-response.model';
import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ChatService } from '../services/chat.service';
import { ReplaySubject } from 'rxjs';
import { createLabeledTextarea } from 'ckeditor5/src/ui';
import { HttpException } from 'src/shared/models/http-exception.model';
import { ToastrService } from 'ngx-toastr';
import { ToastrExtension } from 'src/shared/toastr.extension';
import { Offset } from 'src/shared/offset.model';
import { environment as env } from 'src/environments/environment';

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

  public destroy$ = new ReplaySubject<boolean>();

  constructor(
    private router: Router,
    private chat: ChatService,
    private cdr: ChangeDetectorRef,
    private toastr: ToastrService
  ) {
  }

  ngOnInit(): void {
    this.chat.currentChat$.subscribe(chatId => {
      this.chatId = chatId;
      this.cdr.detectChanges();
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

  ngOnDestroy(): void {
    this.destroy$.next(true);
    this.destroy$.complete();
  }
}
