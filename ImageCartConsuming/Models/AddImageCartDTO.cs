namespace ImageCartConsuming.Models
{
    public class AddImageCartDTO
    {
        public string ImageName { get; set; }
        public IFormFile Images { get; set; }
    }
}
