using NextCuisineApi.Tools;

namespace NextCuisineApi.Models
{
    public class GuestUploadFile
    {
        public string Id { get; set; } = DataTools.RandomString(12);
        public string Filename { get; set; } = String.Empty;
        public string FilenameS3 { get; set; } = String.Empty;
        public DateTime UploadDateTime { get; set; } = DateTime.Now;
    }
}
