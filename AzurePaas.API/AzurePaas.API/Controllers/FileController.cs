using Microsoft.AspNetCore.Mvc;

namespace AzurePaas.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public FileController(IWebHostEnvironment env)
        {
            _env = env;
        }
        [HttpPost(Name = "UploadImage")]
        public async Task<IActionResult> Get([FromForm] Content content)
        {
            try
            {

                var fileService = new FileService();
                await fileService.ImageUpload(content.File, _env);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
    }
}
