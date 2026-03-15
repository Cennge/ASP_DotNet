using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asp_all.Services.Kdf
{
    internal interface IKdfService
    {
        String Dk(String salt, String password);
    }
}
