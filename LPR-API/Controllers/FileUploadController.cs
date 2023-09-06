using Serilog;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Prom.LPR.Api.Models;
using Prom.LPR.Api.Utils;
using Google.Cloud.Storage.V1;
using Prom.LPR.Api.Kafka;
using System.Text.Json;

namespace Prom.LPR.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IConfiguration cfg;
        private string lprBaseUrl = "";
        private string lprPath = "";
        private string lprAuthKey = "";
        private string imagesBucket = "";
        private string topic = "";
        private string kafkaHost = "";
        private string kafkaPort = "";
        private Producer<MKafkaMessage> producer;

        public FileUploadController(IConfiguration configuration)
        {
            cfg = configuration;
            imagesBucket = ConfigUtils.GetConfig(cfg, "LPR:bucket");
            lprBaseUrl = ConfigUtils.GetConfig(cfg, "LPR:lprBaseUrl");
            lprPath = ConfigUtils.GetConfig(cfg, "LPR:lprPath");
            lprAuthKey = ConfigUtils.GetConfig(cfg, "LPR:lprAuthKey");

            topic = ConfigUtils.GetConfig(cfg, "Kafka:topic");
            kafkaHost = ConfigUtils.GetConfig(cfg, "Kafka:host");
            kafkaPort = ConfigUtils.GetConfig(cfg, "Kafka:port");

            Log.Information($"LPR URL=[{lprBaseUrl}], LPR Path=[{lprPath}]");
            Log.Information($"Topic=[{topic}], Kafka Host=[{kafkaHost}], Kafka Port=[{kafkaPort}]");

            producer = new Producer<MKafkaMessage>(kafkaHost, kafkaPort);
        }

        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            Uri baseUri = new Uri(lprBaseUrl);
            client.BaseAddress = baseUri;
            client.Timeout = TimeSpan.FromMinutes(0.05);

            return client;
        }

        private HttpRequestMessage GetRequestMessage()
        {
            //Bearer Authentication
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, lprPath);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", lprAuthKey);
            var productValue = new ProductInfoHeaderValue("lpr-api", "1.0");
            requestMessage.Headers.UserAgent.Add(productValue);

            return requestMessage;
        }

        private void PublishMessage(MKafkaMessage data)
        {
            producer.Produce(data, topic);
        }

        private string UploadFile(string localPath, string org, string folder) 
        {
            var objectName = Path.GetFileName(localPath);
            string objectPath = $"{org}/{folder}/{objectName}";
            string gcsPath = $"gs://{imagesBucket}/{objectPath}";

            Log.Information($"Uploading file [{localPath}] to [{gcsPath}]");

            StorageClient storageClient = StorageClient.Create();
            using (var f = System.IO.File.OpenRead(localPath))
            {
                storageClient.UploadObject(imagesBucket, $"{objectPath}", null, f);
            }

            return gcsPath;
        }

        private string LPRAnalyzeFile(string imagePath)
        {
            var client = GetHttpClient();
            var requestMessage = GetRequestMessage();

            using var stream = System.IO.File.OpenRead(imagePath);
            using var content = new MultipartFormDataContent
            {
                { new StreamContent(stream), "image", imagePath }
            };

            requestMessage.Content = content;
            var task = client.SendAsync(requestMessage);
            var response = task.Result;

            var lprResult = "";
            try
            {
                response.EnsureSuccessStatusCode();
                string responseBody = response.Content.ReadAsStringAsync().Result;

                lprResult = responseBody;
                Console.WriteLine($"{responseBody}");
            }
            catch (Exception e)
            {
                string responseBody = response.Content.ReadAsStringAsync().Result;
                Log.Error(responseBody);
                Log.Error(e.Message);
            }

            return lprResult;
        }

        private MLPRResult? GetLPRObject(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var obj = JsonSerializer.Deserialize<MLPRResult>(json, options);
            return obj;
        }

        [HttpPost]
        [Route("org/{id}/action/UploadVehicleImage")]
        public IActionResult UploadVehicleImage(string id, [FromForm] MImageUploaded img)
        {
            var image = img.Image;
            if (image == null)
            {
                Log.Information($"No uploaded file available");
                return NotFound();
            }

            var ts = DateTime.Now.ToString("yyyyMMddhhmmss");
            var tmpFile = $"/tmp/{ts}.{image.FileName}";
            using (var fileStream = new FileStream(tmpFile, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }

            Log.Information($"Uploaded file [{image.FileName}], saved to [{tmpFile}]");
            var msg = LPRAnalyzeFile(tmpFile);
            var lprObj = GetLPRObject(msg);


            var dateStamp = DateTime.Now.ToString("yyyyMMddhh");
            var folder = $"{dateStamp}";
            var storagePath = UploadFile(tmpFile, id, folder);
            
            var storageObj = new MStorageData() 
            {
                StoragePath = storagePath
            };

            var data = new MKafkaMessage() 
            {
                LprData = lprObj,
                StorageData = storageObj
            };

            PublishMessage(data);

            return Ok(msg);
        }
    }
}
