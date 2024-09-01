import { PostInfo } from 'src/shared/models/post-info.model'
import { Post } from 'src/shared/models/post-model'
import { User } from 'src/shared/models/user.model'

import { Topic } from './topic.model'

export type TopicDetail = {
    topic: Topic
    posts: PostInfo
    post: Post
    sender: User
}
