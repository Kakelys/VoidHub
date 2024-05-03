import { ForumResponse } from './forum-response.model';
import { Section } from './section.model';

export interface SectionFullResponse {
  section: Section;
  forums: ForumResponse[];
}
