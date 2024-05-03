import { Forum } from './forum.model';
import { Section } from './section.model';

export interface SectionResponse {
  section: Section;
  forums: Forum[];
}
