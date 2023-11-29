namespace NextCuisine.Models
{
    public class GuestUpload
    {
        public string Id { get; set; } = DataTools.RandomString(12);
        public string Username { get; set; }
        public string Title { get; set; } = "Untitled: " + DateTime.Now.ToLongDateString();
    }
}
