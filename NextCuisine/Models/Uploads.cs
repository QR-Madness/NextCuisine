using System.Collections.ObjectModel;

namespace NextCuisine.Models
{
    public class Uploads
    {
        public DateTime SyncDateTime { get; } = DateTime.Now;
        public Collection<GuestUpload> GuestUploads { get; set; } = new Collection<GuestUpload>();
    }
}
