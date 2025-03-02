using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CnSharp.Updater.Util
{
    public class HttpUtil
    {
        public static async Task CheckUrl(string url)
        {
            if (!IsValidUrl(url))
                throw new ArgumentException("The provided URL is not a valid URL.");
            if (!await IsUrlAccessible(url))
                throw new HttpRequestException("The provided URL is not accessible.");
        }

        public static bool IsValidUrl(string url)
        {
            var pattern = @"^(http|https)://";
            return Regex.IsMatch(url, pattern);
        }

        public static async Task<bool> IsUrlAccessible(string url)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(url);
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
