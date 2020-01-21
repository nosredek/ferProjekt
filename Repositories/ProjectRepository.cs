using FerProjekt.Domain;
using FerProjekt.Domain.Tables;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FerProjekt.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ProjectContext _context;
        private const int pageSize = 12;

        public ProjectRepository(ProjectContext context)
        {
            _context = context;
        }

        public async Task AddImage(Images image)
        {
            await _context.AddAsync(image);
            await _context.SaveChangesAsync();
        }

        public async Task<Images> GetImageById(int id)
        {
            return await _context.Images.Include(i => i.TaggedFaces)
                                        .SingleOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Images> GetImageByLink(string link)
        {
            return await _context.Images.Include(i => i.TaggedFaces)
                                        .SingleOrDefaultAsync(i => i.ImageLink == link);
        }

        public async Task<IEnumerable<Images>> GetImagesOnPage(int pageNum)
        {
            return await _context.Images.Include(i => i.TaggedFaces)
                                        .Skip((pageNum-1) * pageSize)
                                        .Take(pageSize)
                                        .ToListAsync();
        }

        public async Task<int> GetPageCount()
        {
            return 1+((await _context.Images.CountAsync()) / pageSize);
        }

        public async Task DeleteImage(int id)
        {
            var image = await GetImageById(id);
            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
        }
    }
}