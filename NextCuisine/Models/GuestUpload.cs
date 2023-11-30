using System.Collections.ObjectModel;
using Microsoft.VisualBasic;
using NextCuisine.Tools;

namespace NextCuisine.Models
{
    public class GuestUpload
    {
        public string Id { get; set; } = DataTools.RandomString(12);
        public string OwnerUid { get; set; } = String.Empty;
        public string Visibility { get; set; } = "Public";
        public DateTime LastEditTime { get; set; } = DateTime.Now;
        public DateTime UploadDate { get; set; } = DateTime.Now;
        public string Title { get; set; } = "Untitled: " + DateTime.Now.ToLongDateString();
        public string ShortDescription { get; set; } = String.Empty;
        public string Content { get; set; } = String.Empty;
        public Collection<GuestUploadFile> Files { get; set; } = new Collection<GuestUploadFile>();
        public Dictionary<string, string> AdditionalContent { get; set; } = new Dictionary<string, string>()
        {
            ["UploadStyle"] = "General"
        };
    }
}
