import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { take } from 'rxjs';

@Component({
  selector: 'app-delete',
  templateUrl: './delete.component.html',
  styleUrls: ['./delete.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    TranslateModule
  ]
})
export class DeleteComponent {
  @Input()
  confirmContent: string = '';

  @Output()
  onConfirm = new EventEmitter();

  @Input()
  height: string = '30px';

  @Input()
  width: string = '20px';

  constructor(trans: TranslateService) {
    trans.get('lables.are-you-sure')
    .pipe(take(1))
    .subscribe(str => {
      this.confirmContent = str;
    })
  }

  confirmClick() {
    this.onConfirm.emit();
  }
}
