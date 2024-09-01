import { Post } from 'src/shared/models/post-model'
import { User } from 'src/shared/models/user.model'

import { Forum } from './forum.model'
import { Topic } from './topic.model'

export type TopicInfo = {
    topic: Topic
    forum: Forum
    post: Post
    sender: User
}
