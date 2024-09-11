import { Post } from 'src/app/common/models/post-model'

import { User } from '../../auth'
import { Forum } from './forum.model'
import { Topic } from './topic.model'

export type TopicInfo = {
    topic: Topic
    forum: Forum
    post: Post
    sender: User
}
