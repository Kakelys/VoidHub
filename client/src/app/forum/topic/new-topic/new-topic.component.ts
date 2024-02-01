import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgFormExtension } from 'src/shared/ng-form.extension';
import { TopicService } from '../../services/topic.service';
import Editor from 'ckeditor5/build/ckeditor'
import { NameService } from '../../services/name.service';
import { Name } from '../../models/names.model';
import { ToastrExtension } from 'src/shared/toastr.extension';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-new-topic',
  templateUrl: './new-topic.component.html',
  styleUrls: ['./new-topic.component.css']
})
export class NewTopicComponent implements OnInit {
  editor = Editor as {
    create: any;
  }

  forumId;
  errorMessages: string[] = [];
  content = '';

  @Output()
  close = new EventEmitter();

  @Output()
  created = new EventEmitter();

  names: Name[] = null;

  constructor(
    route: ActivatedRoute,
    private router: Router,
    private topicService: TopicService,
    private nameService: NameService,
    private toastr: ToastrService) {
    route.params.subscribe(params => {
      this.forumId = params['id']
    })
  }

  ngOnInit(): void {
    this.nameService.getForums().subscribe({
      next: (names: Name[]) => {
        this.names = names;
      },
      error: errs => {ToastrExtension.handleErrors(this.toastr, errs)}
    })
  }

  onSubmit(form: NgForm) {
    this.errorMessages = [];

    if(form.invalid) {
      NgFormExtension.markAllAsTouched(form);
      return;
    }

    this.topicService.createTopic(form.value).subscribe({
      next: _ => {
        this.created.emit();
      },
      error: errs => {
        this.errorMessages = errs
      }
    });
  }

  onCancel() {
    this.close.emit();
  }
}
