import { ActivatedRoute, Router } from '@angular/router';
import { EmailService } from './../email.service';
import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { HttpException } from 'src/shared/models/http-exception.model';
import { ToastrExtension } from 'src/shared/toastr.extension';
import { AuthService } from 'src/app/auth/auth.service';
import { take } from 'rxjs';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css']
})
export class ConfirmEmailComponent implements OnInit {

  confirmed: boolean = false;

  limitNames = environment.limitNames;

  constructor(
    private emailService: EmailService,
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private auth: AuthService
  ) {}


  ngOnInit(): void {
    this.emailService.confirmEmail(this.route.snapshot.params["token"])
    .subscribe({
      next: _ => {
        this.confirmed = true;

        this.auth.user$
        .pipe(take(1))
        .subscribe(user => {
          if(!user)
            return;

          user.isEmailConfirmed = true;
          this.auth.updateUser(user);
        })
      },
      error: (err: HttpException) => {
        ToastrExtension.handleErrors(this.toastr, err.errors);
      }
    })
  }
}
