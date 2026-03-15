using System.Text;

namespace asp_all.Services.Hash
{
    public class ShaHashService : IHashService
    {
        public string Digest(string input)
        {
            return Convert.ToHexString(
                System.Security.Cryptography.SHA1.HashData(
                    Encoding.UTF8.GetBytes(input)
                )
            );
        }
    }
}
