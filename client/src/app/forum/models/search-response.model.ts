import { SearchElement } from "./search-element.model";
import { TopicInfo } from "./topic-info.model";

export interface SearchResponse {
  searchCount: number;
  data: SearchElement[];
}
