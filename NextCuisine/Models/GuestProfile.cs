using System.Collections.ObjectModel;

namespace NextCuisine.Models
{
    public class GuestProfile
    {
        public string Uid { get; set; }
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public Collection<GuestUpload> ProfileUploads { get; set; } = new Collection<GuestUpload>();
        public Collection<GuestUpload> PrivateUploads { get; set; } = new Collection<GuestUpload>();
    }
}
