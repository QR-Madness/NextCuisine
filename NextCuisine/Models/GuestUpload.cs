using System.Collections.ObjectModel;
using Microsoft.VisualBasic;

namespace NextCuisine.Models
{
    public class GuestUpload
    {
        public string Id { get; set; } = DataTools.RandomString(12);
        public string OwnerUid { get; set; } = String.Empty;
        public string Title { get; set; } = "Untitled: " + DateTime.Now.ToLongDateString();
        public DateTime UploadDate { get; set; } = DateTime.Now;
        public Collection<GuestUploadFile> Files { get; set; } = new Collection<GuestUploadFile>();
        public DateTime LastEditTime { get; set; } = DateTime.Now;
    }
}
