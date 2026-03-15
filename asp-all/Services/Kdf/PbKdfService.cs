using asp_all.Services.Hash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asp_all.Services.Kdf
{
    internal class PbKdfService(IHashService hashService) : IKdfService
    {
        private readonly IHashService _hashService = hashService;

        public string Dk(string salt, string password)
        {
            String t = _hashService.Digest(salt + password);
            for (int i = 0; i < 100000; i++)
            {
                t = _hashService.Digest(t);
            }
            return t;
        }
    }
}
