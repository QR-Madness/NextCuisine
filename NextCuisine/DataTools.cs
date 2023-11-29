using System.Text;

namespace NextCuisine
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
    }
}
