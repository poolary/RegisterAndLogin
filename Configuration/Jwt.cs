using System.Text;

namespace LoggAutorz.Configuration
{
    public static class JwtTokenConfig
    {
        public static string PrivateKey { get; set; }
        public static byte[] GetKeyBytes()
        {
            return Encoding.ASCII.GetBytes(PrivateKey);
        }
    } }

