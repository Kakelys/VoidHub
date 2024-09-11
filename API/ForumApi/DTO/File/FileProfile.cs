using AutoMapper;

namespace ForumApi.DTO.DFile;

public class FileProfile : Profile
{
    public FileProfile()
    {
        CreateMap<FileDto, Data.Models.File>();
    }
}