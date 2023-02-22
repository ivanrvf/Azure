using System;
using System.Configuration;

namespace RandomPrograms
{
    class IPConversion
    {
        
        static void Main(string[] args)
        {
            var ipAddress = ConfigurationManager.AppSettings["SampleIPv6"].ToString();//Example IPv6
            var ip6 = ipAddress.Split(':');
            var firstBit = "000" + ip6[ip6.Length - 2].ToString(); //padding the part which contains first 2 bits of IPv4 address to ensure we get 4 digit length
            var secondBit = ("000" + ip6[ip6.Length - 1].ToString()); //padding the part which contains last 2 bits of IPv4 address to ensure we get 4 digit length
            firstBit = firstBit.Substring(firstBit.Length - 4, 4); //take last 4 digit of padded part
            secondBit = secondBit.Substring(secondBit.Length - 4, 4); //take last 4 digit of padded part
            var ipv4 = Convert.ToInt32(firstBit.Substring(0, 2), 16).ToString() + ":" + Convert.ToInt32(firstBit.Substring(2, 2), 16).ToString() + ":" + Convert.ToInt32(secondBit.Substring(0, 2), 16).ToString() + ":" + Convert.ToInt32(secondBit.Substring(2, 2), 16).ToString();
            System.Diagnostics.Debug.WriteLine("Hex to integer check: " + ipv4); // Prints in the output window of Debug.
        }
    }
}
