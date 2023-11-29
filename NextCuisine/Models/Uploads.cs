using System.Collections.ObjectModel;

namespace NextCuisine.Models
{
    public class Uploads
    {
        public Collection<GuestUpload> GuestUploads { get; set; } = new Collection<GuestUpload>();
    }
}
