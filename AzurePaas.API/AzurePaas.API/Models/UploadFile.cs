using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace AzurePaas.API.Models
{
    public class UploadFile
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("fileName")]
        public string FileName { get; set; }
        [JsonProperty("fileType")]
        public string FileType { get; set; }
        public bool IsProcessed { get; set; }
    }
}
