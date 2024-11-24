namespace Transport.Model
{
    public class UploadAadharPanRequest
    {
        public IFormFile AadharCard { get; set; }
        public IFormFile PanCard { get; set; }
    }
}
