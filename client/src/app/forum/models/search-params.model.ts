import { SearchSort } from "../search/search-sort.enum";

export interface SearchParams {
  sort: SearchSort | string | null;
  withPostContent: boolean;
  onlyDeleted: boolean;
  partialTitle: boolean;
}
