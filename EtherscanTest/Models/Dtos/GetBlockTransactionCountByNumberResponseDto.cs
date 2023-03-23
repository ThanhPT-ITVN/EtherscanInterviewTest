using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtherscanTest.Models.Dtos
{
    public class GetBlockTransactionCountByNumberResponseDto
    {
        public string jsonrpc { get; set; }
        public int id { get; set; }
        public string result { get; set; }
    }
}
