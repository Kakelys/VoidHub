import { Forum } from './forum.model'
import { LastTopic } from './last-topic.model'

export type ForumResponse = {
    forum: Forum
    postsCount: number
    topicsCount: number
    lastTopic: LastTopic | null
}
