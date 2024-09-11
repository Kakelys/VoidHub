import { User } from 'src/app/modules/auth'
import { Topic, Forum } from 'src/app/modules/forum'
import { Post } from './post-model'

export type PostInfo = {
    post: Post
    sender: User
    topic: Topic
    forum: Forum
}
