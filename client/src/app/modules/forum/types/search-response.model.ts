import { SearchElement } from './search-element.model'

export type SearchResponse = {
    searchCount: number
    data: SearchElement[]
}
