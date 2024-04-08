import { TopicInfo } from "./topic-info.model";

export interface SearchResponse {
  searchCount: number;
  data: TopicInfo[];
}
