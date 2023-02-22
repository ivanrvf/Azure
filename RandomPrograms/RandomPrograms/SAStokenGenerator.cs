using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RandomPrograms
{
    class SAStokenGenerator
    {
        static void Main(string[] args)
        {
            try
            {
                string keyName = "RootManageSharedAccessKey";
                string resourceUri = "sbnsdemo.servicebus.windows.net/blobcreate-events-webhook";
                string key = ConfigurationManager.AppSettings["SBprimarykey"].ToString();
                TimeSpan sinceEpoch = DateTime.UtcNow - new DateTime(1970, 1, 1);
                var week = 60 * 60 * 24 * 365 * 15;
                var expiry = Convert.ToString((long)sinceEpoch.TotalSeconds + week); // Changed from int to long to accomodate higher expiry period like 15 years.
                string stringToSign = HttpUtility.UrlEncode(resourceUri) + "\n" + expiry;
                HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
                var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
                var sasToken = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}", HttpUtility.UrlEncode(resourceUri), HttpUtility.UrlEncode(signature), expiry, keyName);
                System.Diagnostics.Debug.WriteLine(sasToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Encountered an error while creating token ", ex);
            }
        }
    }
}
