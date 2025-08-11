namespace ImageCartConsuming.Models
{
    public class FetchImageCartDTO
    {
        public int ImageId { get; set; }
        public string ImageName { get; set; }
        public string Images { get; set; }
        public byte isDeleted { get; set; }
    }
}
