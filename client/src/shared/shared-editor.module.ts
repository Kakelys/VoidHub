import { NgModule } from "@angular/core";
import { CKEditorModule } from "@ckeditor/ckeditor5-angular";

@NgModule({
  imports: [
    CKEditorModule,
  ],
  exports: [
    CKEditorModule,
  ]
})
export class SharedEditorModule{}
