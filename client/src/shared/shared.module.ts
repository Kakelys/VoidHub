import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { CKEditorModule } from "@ckeditor/ckeditor5-angular";
import { PostEditorComponent } from "src/app/forum/post/post-editor/post-editor.component";
import { LimitLoaderComponent } from "src/app/limitter/limit-loader/limit-loader.component";
import { TimezonePipe } from "./timemezone.pipe";

@NgModule({
  declarations: [
    LimitLoaderComponent,
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
    TimezonePipe
  ]
})
export class  SharedModule{}
