using ForumApi.Data.Models;
using ForumApi.Data.Repository.Interfaces;
using ForumApi.DTO.DName;
using ForumApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ForumApi.Services
{
    public class NamesService(IRepositoryManager rep) : INamesService
    {
        public async Task<List<Name>> GetForums(bool includeHidden = false)
        {
            var res = new List<Name>();
            IQueryable<Section> query;
            if(includeHidden)
                query = rep.Section.Value.FindAll();
            else
                query = rep.Section.Value
                    .FindByCondition(s => s.IsHidden == false);

            var sections = await query
                .Include(s => s.Forums.Where(f => f.DeletedAt == null))
                .OrderBy(s => s.OrderPosition)
                .ToListAsync();
            
            sections.ForEach(s =>
            {
                res.Add(new(){Title = s.Title, IsSelectable = false});
                s.Forums.ForEach(f => res.Add(new(){Id = f.Id, Title = f.Title, IsSelectable = true}));
            });

            return res;
        }

        public Task<List<Name>> GetSections()
        {
            return rep.Section.Value
                .FindAll()
                .OrderBy(s => s.OrderPosition)
                .Select(s => new Name
                {
                    Id = s.Id,
                    Title = s.Title,
                    IsSelectable = true
                }).ToListAsync();
        }
    }
}