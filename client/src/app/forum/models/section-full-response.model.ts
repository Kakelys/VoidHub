import { ForumResponse } from './forum-response.model'
import { Section } from './section.model'

export type SectionFullResponse = {
    section: Section
    forums: ForumResponse[]
}
