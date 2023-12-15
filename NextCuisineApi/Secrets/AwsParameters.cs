using Amazon;

namespace NextCuisineApi.Secrets
{
    public class AwsParameters
    {
        public static KeyValuePair<string, string> Iam()
        {
            // Dev User for project
            // Todo KMS integration
            return new KeyValuePair<string, string>("AKIAVJLLO4FSSGYDU2UF", "EYkXdK5QvUVW/IwltM+sTWLl/tveCK4LA9VLOxKC");
        }
    }
}