import { SearchSort } from '../../search/types/search-sort.enum'

export type SearchParams = {
    sort: SearchSort | string | null
    forumId: number | null
    withPostContent: boolean
    onlyDeleted: boolean
    partialTitle: boolean
}
