using System.Text;

namespace asp_all.Services.Hash
{
    public class Md5HashService : IHashService
    {
        public string Digest(string input)
        {
            return Convert.ToHexString(
                System.Security.Cryptography.MD5.HashData(
                    Encoding.UTF8.GetBytes(input)
                )
            );
        }
    }
}
