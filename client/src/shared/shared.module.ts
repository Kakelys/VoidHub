import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { LimitLoaderComponent } from "src/app/limitter/limit-loader/limit-loader.component";
import { TimezonePipe } from "./timemezone.pipe";
import { ConfirmComponent } from "src/app/shared/confirm/confirm.component";

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
