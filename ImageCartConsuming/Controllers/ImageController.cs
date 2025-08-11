using ImageCartConsuming.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ImageCartConsuming.Controllers
{
    public class ImageController : Controller
    {
        HttpClient httpClient;
        public ImageController()
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            httpClient = new HttpClient(clientHandler);
        }
        public IActionResult AddImage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddImage(AddImageCartDTO data)
        {
            using var formContent = new MultipartFormDataContent();
            formContent.Add(new StringContent(data.ImageName ?? ""), "ImageName");

            if (data.Images != null)
            {
                var fileStreamContent = new StreamContent(data.Images.OpenReadStream());
                fileStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(data.Images.ContentType);
                formContent.Add(fileStreamContent, "Images", data.Images.FileName);
            }

            string url = "https://localhost:7015/api/Imgages/AddImages";
            var response = httpClient.PostAsync(url, formContent).Result;

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Image Added Successfully";
                return RedirectToAction("AddImage");
            }

            return RedirectToAction("Index");
        }

        public IActionResult FetchImages()
        {
            List<FetchImageCartDTO> e = new List<FetchImageCartDTO>();
            string url = "https://localhost:7015/api/Imgages/FetchImages";
            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObject<List<FetchImageCartDTO>>(json);
                if (obj != null)
                {
                    e = obj;
                }
            }
            return View(e);
        }

        [HttpPost] // Keep POST for MVC, even though API uses DELETE
        public IActionResult SoftDeleteImages([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return BadRequest("No IDs provided.");

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri("https://localhost:7015/api/Imgages/delete-multiple"),
                Content = new StringContent(JsonConvert.SerializeObject(ids), Encoding.UTF8, "application/json")
            };

            var response = httpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = response.Content.ReadAsStringAsync().Result; // debug info
                return StatusCode((int)response.StatusCode, errorContent);
            }

            return Ok(new { message = "Images deleted successfully" });
        }



    }
}

