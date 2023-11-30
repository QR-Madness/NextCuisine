using Amazon;

namespace NextCuisine.Config
{
    public class AwsParameters
    {
        public static KeyValuePair<string, string> Iam()
        {
            // Dev User for project
            // Todo KMS integration
            return new KeyValuePair<string, string>("AKIAVJLLO4FSTH4776W4", "Y7seEeMhOVfaxyxpOtbrAFpe0dDnK594gLPJJShY");
        }
    }
}
