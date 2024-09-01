import { SearchSort } from '../search/search-sort.enum'

export type SearchParams = {
    sort: SearchSort | string | null
    forumId: number | null
    withPostContent: boolean
    onlyDeleted: boolean
    partialTitle: boolean
}
