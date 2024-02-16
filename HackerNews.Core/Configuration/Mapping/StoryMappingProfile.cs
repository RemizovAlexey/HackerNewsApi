using AutoMapper;
using HackerNews.Core.Models;
using HackerNews.Data.Models;

namespace HackerNews.Core.Configuration.Mapping
{
    public class StoryMappingProfile: Profile
    {
        public StoryMappingProfile()
        {
            CreateMap<StoryDto, Story>()
            .ForMember(x => x.Title, opt => opt.MapFrom(x => x.Title))
            .ForMember(x => x.Uri, opt => opt.MapFrom(x => x.Url))
            .ForMember(x => x.PostedBy, opt => opt.MapFrom(x => x.By))
            .ForMember(x => x.Time, opt => opt.MapFrom(x => 
                new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(x.Time)))
            .ForMember(x => x.Score, opt => opt.MapFrom(x => x.Score))
            .ForMember(x => x.CommentCount, opt => opt.MapFrom(x => x.Descendants));
        }
    }
}
