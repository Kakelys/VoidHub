export const environment = {
  commenthDepthLimit: 5,
  maxAvatarSize: 1048576,
  baseAPIUrl: 'http://localhost:5000/api',
  resourceURL: 'http://localhost:5000',
  production: false,
  localizationPrefix: "./assets/i18n/l.",
  limitNames: {
    limitParam: 'X-Limit-Param',
    nameParam: 'X-Name-Param',
    skipParam: 'X-Limit-Skip',

    topicLoad: 'topic-load',
    forumLoad: 'forum-load',
    topicsLoad: 'topics-load',
    sectionsLoad: 'sections-load',
    chatInfoLoad: 'chat-info-load',
    chatLoads: 'chatsLoad',
    chatLoadMsgs: 'chatLoadMsgs',
    loadProfilePosts: 'loadProfilePosts',
    loadProfileTopics: 'loadProfileTopics',
    loadComments: 'loadComments'
  },
}
