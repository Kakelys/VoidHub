import { StringExtension } from "src/shared/string.extension";

export interface SearchParams {
  sort: string;
  withPostContent: boolean;
  onlyDeleted: boolean;



  // public Equals(other: SearchParams): boolean {
  //   return (
  //     this.sort == other.sort &&
  //     this.withPostContent == other.withPostContent &&
  //     this.onlyDeleted == other.onlyDeleted
  //   );
  // }


}
