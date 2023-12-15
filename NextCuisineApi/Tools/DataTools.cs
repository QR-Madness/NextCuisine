using Amazon.DynamoDBv2.Model;
using System.Text;

namespace NextCuisineApi.Tools
{
    public class DataTools
    {
        public static string RandomString(int length)
        {
            const string characters = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder randomString = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(characters.Length);
                randomString.Append(characters[index]);
            }
            return randomString.ToString();
        }

        public static Dictionary<string, string> ConvertAttributeValuesToDictionary(Dictionary<string, AttributeValue> attributeValues)
        {
            return attributeValues.ToDictionary(entry => entry.Key, entry => entry.Value.S);
        }

        public static string GetValueOrDefault(Dictionary<string, AttributeValue> attributeValues, string key)
        {
            return attributeValues.TryGetValue(key, out var attributeValue) ? attributeValue.S : string.Empty;
        }

        public static DateTime GetDateTimeValueOrDefault(Dictionary<string, AttributeValue> attributeValues, string key)
        {
            return attributeValues.TryGetValue(key, out var attributeValue) ? DateTime.Parse(attributeValue.S) : DateTime.Today;
        }
    }
}
