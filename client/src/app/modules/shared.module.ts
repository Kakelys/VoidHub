import { CommonModule } from '@angular/common'
import { NgModule } from '@angular/core'
import { FormsModule } from '@angular/forms'
import { RouterModule } from '@angular/router'

import { TimezonePipe } from '../../shared/timemezone.pipe'

import { LimitLoaderComponent } from '../common/utils'
import { ConfirmComponent } from './forum'

@NgModule({
    declarations: [LimitLoaderComponent, ConfirmComponent],
    imports: [FormsModule, CommonModule, RouterModule, TimezonePipe],
    exports: [
        FormsModule,
        CommonModule,
        RouterModule,
        LimitLoaderComponent,
        TimezonePipe,
        ConfirmComponent,
    ],
})
export class SharedModule {}
