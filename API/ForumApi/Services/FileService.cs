using AutoMapper;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.DFile;
using ForumApi.Services.Interfaces;
using ForumApi.Utils.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Services
{
    public class FileService(IRepositoryManager rep, IMapper mapper) : IFileService
    {
        public async Task<List<FileDto>> Get(int? postId)
        {
            return await rep.File.Value.FindByCondition(f => f.PostId == postId)
                .Select(f => new FileDto(f))
                .ToListAsync();
        }

        public async Task<List<FileDto>> Get(int[] ids) 
        {
            var set = ids.ToHashSet();
            return await rep.File.Value
                .FindByCondition(f => set.Contains(f.Id))
                .Select(f => new FileDto(f))
                .ToListAsync();
        }

        public async Task<List<FileDto>> GetInvalid()
        {
            return await rep.File.Value.FindByCondition(f => f.PostId == null)
                .Select(f => new FileDto(f))
                .ToListAsync();
        }

        public async Task<FileDto> Create(FileDto file)
        {
            var newFile = rep.File.Value.Create(mapper.Map<Data.Models.File>(file));
            await rep.Save();

            return new FileDto(file);
        }

        public async Task<List<FileDto>> Delete(int[] ids)
        {
            var set = ids.ToHashSet();
            var entities = await rep.File.Value.FindByCondition(f => set.Contains(f.Id)).ToListAsync();
            
            if(set.Count != entities.Count)
                throw new BadRequestException("Failed to find one or more files");

            rep.File.Value.DeleteMany(entities);
            await rep.Save();

            return mapper.Map<List<FileDto>>(entities);
        }

        public async Task Update(int[] ids, int? postId)
        {
            var set = ids.ToHashSet();
            var entities = await rep.File.Value.FindByCondition(f => set.Contains(f.Id), true).ToListAsync();
            
            if(set.Count != entities.Count)
                throw new BadRequestException("Failed to find one or more files");

            foreach(var el in entities)
            {
                el.PostId = postId;
            }

            await rep.Save();
        }
    }
}