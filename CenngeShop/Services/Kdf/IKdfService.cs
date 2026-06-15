using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenngeShop.Services.Kdf
{
    public interface IKdfService
    {
        String Dk(String salt, String password);
    }
}
