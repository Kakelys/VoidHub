namespace ForumApi.DTO.DFile;

public class FileDto : Data.Models.File
{
    public FileDto() { }

    public FileDto(Data.Models.File file)
    {
        Id = file.Id;
        PostId = file.PostId;
        Path = file.Path;
        AccountId = file.AccountId;
    }
}