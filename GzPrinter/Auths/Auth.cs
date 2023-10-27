using System;

namespace GzPrinter
{
    internal class Auth
    {
        public string Token { get; set; }
        public string Domain { get; set; }
        public string MacAddress { get; set; }
        public DateTime Expired { get; set; }

        public bool IsExpired()
        {
            return Expired < DateTime.Now;
        }
    }
}
