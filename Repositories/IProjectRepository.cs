using FerProjekt.Domain.Tables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FerProjekt.Repositories
{
    public interface IProjectRepository
    {
        Task AddImage(Images image);

        Task<IEnumerable<Images>> GetImagesOnPage(int pageNum);

        Task<Images> GetImageById(int id);

        Task<Images> GetImageByLink(string link);

        Task<int> GetPageCount();

        Task DeleteImage(int id);
    }
}