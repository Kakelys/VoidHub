import { Forum } from 'src/app/forum/models/forum.model'
import { Topic } from 'src/app/forum/models/topic.model'

import { Post } from './post-model'
import { User } from './user.model'

export type PostInfo = {
    post: Post
    sender: User
    topic: Topic
    forum: Forum
}
