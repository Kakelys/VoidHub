import { PostInfo } from 'src/app/common/models/post-info.model'
import { Post } from 'src/app/common/models/post-model'

import { User } from '../../auth'
import { Topic } from './topic.model'

export type TopicDetail = {
    topic: Topic
    posts: PostInfo
    post: Post
    sender: User
}
