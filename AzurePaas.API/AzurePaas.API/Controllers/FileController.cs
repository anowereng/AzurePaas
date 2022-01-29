using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using AzurePaas.API.Models;
using AzurePaas.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace AzurePaas.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }


        [HttpPost]
        [Route("UploadImage")]
        public async Task<IActionResult> Post(IFormFile content)
        {
            try
            {
                await _fileService.ImageUpload(content);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Route("GetUploadImage")]
        [HttpGet]
        public async Task<IActionResult> GetUploadImage(string fileName)
        {
            var imgBytes = await _fileService.Get(fileName);
            return File(imgBytes, "image/webp");
        }

        [Route("Download")]
        [HttpGet]
        public async Task<IActionResult> Download(string fileName)
        {
            var imagBytes = await _fileService.Get(fileName);
            return new FileContentResult(imagBytes, "application/octet-stream")
            {
                FileDownloadName = Guid.NewGuid().ToString() + ".webp",
            };
        }

        [Route("Delete")]
        [HttpGet]
        public async Task<IActionResult> Delete(string fileName)
        {
            var result = await _fileService.Delete(fileName);
            return Ok(result);   
        }

        [Route("AzureQueue")]
        [HttpPost]
       
        public async Task<IActionResult> Post(UploadFile model)
        {
            var connectionQueue = "DefaultEndpointsProtocol=https;AccountName=filestorage2022;AccountKey=j9/e2zF6RHXaDPZJ2KGQ9r94RHu8sviug3CVGWGikc/SDrJtilVO5+gVtDlKR/vv54oy1l4JFrp9ralrsfiddw==;EndpointSuffix=core.windows.net";
            var queueName = "upload-file";
            var queueClient = new QueueClient(connectionQueue, queueName);
            var message =  JsonSerializer.Serialize(model);
            await queueClient.SendMessageAsync(message);
            return Ok();
        }
    }
}
