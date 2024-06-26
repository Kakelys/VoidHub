import { NgModule } from "@angular/core";
import { SearchBarComponent } from "./search-bar/search-bar.component";
import { SearchService } from "../services/search.service";
import { SearchComponent } from "./search/search.component";
import { ErrorMessageListComponent } from "src/app/utils/error-message-list/error-message-list.component";
import { SharedModule } from "src/shared/shared.module";
import { PaginatorComponent } from "../paginator/paginator.component";
import { TopicElementComponent } from "../topic/topic-element/topic-element.component";
import { RouterModule } from "@angular/router";
import { SharedForumModule } from "../shared.forum.module";
import { SharedEditorModule } from "src/shared/shared-editor.module";
import { TranslateModule } from "@ngx-translate/core";

@NgModule({
  declarations: [
    SearchComponent,
  ],
  imports: [
    SharedModule,
    SharedEditorModule,
    SharedForumModule,
    TranslateModule.forChild(),
    SearchBarComponent,
    ErrorMessageListComponent,
    PaginatorComponent,
    TopicElementComponent,
    RouterModule.forChild([
      {path: '', component: SearchComponent}
    ])
  ],
  providers: [
    SearchService,
  ],
  exports: []
})
export class SearchModule {}
