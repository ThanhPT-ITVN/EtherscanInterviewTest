using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtherscanTest.Interfaces
{
    public interface IEtherscanService
    {
        Task<block> GetBlockInfoByBlockNumber(int blockNumber);
        Task<int> GetBlockTransactionCountByBlockNumber(int blockNumber);
        Task<transaction> GetTransactionByBlockNumberAndIndex(block block, int indexOfTransaction);
    }
}
