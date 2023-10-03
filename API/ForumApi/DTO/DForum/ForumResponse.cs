using ForumApi.DTO.DTopic;

namespace ForumApi.DTO.DForum
{
    public class ForumResponse
    {
        public int Id {get;set;}
        public int SectionId {get;set;}
        public string Title {get;set;} = null!;

        public int PostsCount {get;set;}
        public int TopicsCount {get;set;}

        public TopicLast? LastTopic {get;set;}
    }
}