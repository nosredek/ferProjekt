using FerProjekt.Domain.Tables;
using System.Collections.Generic;

namespace FerProjekt.Models
{
    public class ImagePagedResponseModel
    {
        public IEnumerable<Images> Image { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
    }
}