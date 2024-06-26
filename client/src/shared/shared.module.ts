import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { TimezonePipe } from "./timemezone.pipe";
import { ConfirmComponent } from "src/app/shared/confirm/confirm.component";
import { LimitLoaderComponent } from "src/app/utils/limitter/limit-loader/limit-loader.component";

@NgModule({
  declarations: [
    LimitLoaderComponent,
    ConfirmComponent
  ],
  imports:[
    FormsModule,
    CommonModule,
    RouterModule,
    TimezonePipe,
  ],
  exports:[
    FormsModule,
    CommonModule,
    RouterModule,
    LimitLoaderComponent,
    TimezonePipe,
    ConfirmComponent,
  ]
})
export class  SharedModule{}
