import { CKEditorModule } from '@ckeditor/ckeditor5-angular'

import { NgModule } from '@angular/core'

@NgModule({
    imports: [CKEditorModule],
    exports: [CKEditorModule],
})
export class SharedEditorModule {}
