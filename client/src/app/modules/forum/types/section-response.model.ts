import { Forum } from './forum.model'
import { Section } from './section.model'

export type SectionResponse = {
    section: Section
    forums: Forum[]
}
