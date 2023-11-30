using System.Collections.ObjectModel;
using NextCuisine.Tools;

namespace NextCuisine.Models
{
    public class GuestProfile
    {
        public string Uid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public Dictionary<string, string> AdditionalContent { get; set; } = new Dictionary<string, string>()
        {
            ["Favorite food"] = ""
        };
    }
}
