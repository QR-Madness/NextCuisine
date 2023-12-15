using NextCuisineApi.Tools;
using static System.String;

namespace NextCuisineApi.Models
{
    public class GuestUploadFeedback
    {
        public string Id { get; set; } = DataTools.RandomString(4);
        public string OwnerUid { get; set; } = Empty;
        public string OwnerName { get; set; } = Empty;
        public string Content { get; set; } = Empty;
        public string Rating { get; set; } = Empty;
        public DateTime CreationTime { get; set; } = DateTime.Now;
    }
}
