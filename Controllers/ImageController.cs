using FerProjekt.Domain.Tables;
using FerProjekt.Models;
using FerProjekt.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FerProjekt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IProjectRepository _repo;

        public ImageController(IProjectRepository repo)
        {
            _repo = repo;
        }

        // GET: api/Image/getByPageNum?pageNum=1
        [HttpGet("getByPageNum")]
        public async Task<IActionResult> GetImagesForPageAsync([FromQuery] int pageNum = 1)
        {
            if (pageNum < 1 || pageNum > await _repo.GetPageCount())
            {
                return NotFound("pageNum out of range");
            }
            var response = new ImagePagedResponseModel
            {
                Image = await _repo.GetImagesOnPage(pageNum),
                CurrentPage = pageNum,
                PageCount = await _repo.GetPageCount()
            };
            return Ok(response);
        }

        //GET: api/image/getById?id=1
        [HttpGet("getById")]
        public async Task<IActionResult> GetImageById([FromQuery] int id)
        {
            return Ok(await _repo.GetImageById(id));
        }

        //GET: api/image/getByLink
        [HttpPost("getByLink")]
        public async Task<IActionResult> GetImageById([FromBody] string link)
        {
            return Ok(await _repo.GetImageByLink(link));
        }

        // POST: api/Image
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] ImageRequestModel value)
        {
            await _repo.AddImage(new Images
            {
                ImageBlob = Convert.FromBase64String(value.ImageBlob),
                ImageLink = value.ImageLink,
                TaggedFaces = value.TaggedFaces.Select(face => new FaceRectangle
                {
                    Name = face.Name,
                    X1 = face.X1 ?? 0,
                    X2 = face.X2 ?? 0,
                    Y1 = face.Y1 ?? 0,
                    Y2 = face.Y2 ?? 0
                }).ToList()
            });

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImageAsync([FromRoute] int id)
        {
            await _repo.DeleteImage(id);
            return Ok();
        }
    }
}