using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtherscanTest.Interfaces
{
    public interface IDatabaseService
    {
        Task StoreData(block block, List<transaction> transactions);
    }
}
