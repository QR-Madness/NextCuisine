namespace NextCuisine.Models
{
    public class GuestUploadFile
    {
        public string Id { get; set; } = DataTools.RandomString(12);
        public string Filename { get; set; }
    }
}
