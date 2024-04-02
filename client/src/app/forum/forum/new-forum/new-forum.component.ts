import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ForumService } from '../../services/forum.service';
import { NgFormExtension } from 'src/shared/ng-form.extension';
import { Name } from '../../models/names.model';
import { NameService } from '../../services/name.service';
import { ToastrService } from 'ngx-toastr';
import { ToastrExtension } from 'src/shared/toastr.extension';
import { HttpException } from 'src/shared/models/http-exception.model';

@Component({
  selector: 'app-new-forum',
  templateUrl: './new-forum.component.html',
  styleUrls: ['./new-forum.component.css']
})
export class NewForumComponent implements OnInit {

  sectionId: number;
  errorMessages: string[] = [];
  names: Name[] = null;

  constructor(
    private forumService: ForumService,
    private router: Router,
    private route: ActivatedRoute,
    private nameService: NameService,
    private toastr: ToastrService) {

    route.params.subscribe(params => {
      this.sectionId = params['id'];
    })
  }

  ngOnInit(): void {
    this.nameService.getSections().subscribe({
      next: (names: Name[]) => {
        this.names = names;
      },
      error: (err:HttpException) => {
        ToastrExtension.handleErrors(this.toastr, err.errors);
      }
    })
  }

  onSubmit(form: NgForm) {
    this.errorMessages = [];

    if(form.invalid) {
      NgFormExtension.markAllAsTouched(form);

      return;
    }

    this.forumService.createForum(form.value).subscribe({
      next: _ => {
        this.router.navigate(['/forum'], { relativeTo: this.route });
      },
      error: (err: HttpException) => {
        this.errorMessages = err.errors;
      }
    });
  }
}
