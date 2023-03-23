using EtherscanTest.Helpers;
using EtherscanTest.Interfaces;
using EtherscanTest.Models.Dtos;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace EtherscanTest.Implementations
{
    public class EtherscanService : IEtherscanService
    {
        private static string ETHERSCAN_APIKEY = ConfigurationManager.AppSettings["ETHERSCAN_APIKEY"];
        private static string ETHERSCAN_URL_GET_BLOCK_BY_NUMBER = ConfigurationManager.AppSettings["ETHERSCAN_URL_GET_BLOCK_BY_NUMBER"];
        private static string ETHERSCAN_URL_GET_BLOCK_TRANSACTION_COUNT_BY_NUMBER = ConfigurationManager.AppSettings["ETHERSCAN_URL_GET_BLOCK_TRANSACTION_COUNT_BY_NUMBER"];
        private static string ETHERSCAN_URL_GET_TRANSACTION_BY_BLOCKNUMBER_AND_INDEX = ConfigurationManager.AppSettings["ETHERSCAN_URL_GET_TRANSACTION_BY_BLOCKNUMBER_AND_INDEX"];

        private IAppLogger appLogger;
        public EtherscanService(IAppLogger _appLogger)
        {
            appLogger = _appLogger;
        }

        public async Task<block> GetBlockInfoByBlockNumber(int blockNumber)
        {
            appLogger.LogInformation($"Get Block Info By Block Number {blockNumber}");
            var url = String.Format(ETHERSCAN_URL_GET_BLOCK_BY_NUMBER, blockNumber.ToString("X"), ETHERSCAN_APIKEY);
            
            using (var client = new HttpClient(new RetryHandler(new HttpClientHandler())))
            {
                var response = await client.GetAsync(url);
                if (response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var blockInfo = JsonConvert.DeserializeObject<GetBlockByNumberReponseDto>(content);

                        return new block()
                        {
                            blockNumber = Convert.ToInt32(blockInfo.result.number, 16),
                            hash = blockInfo.result.hash,
                            parentHash = blockInfo.result.parentHash,
                            miner = blockInfo.result.miner,
                            gasLimit = Convert.ToInt32(blockInfo.result.gasLimit, 16),
                            gasUsed = Convert.ToInt32(blockInfo.result.gasUsed, 16)
                        };
                    }
                }
                appLogger.LogError(null, $"Can't fetch info for block {blockNumber}");
                throw new Exception($"Can't fetch info for block {blockNumber}");
            }
        }

        public async Task<int> GetBlockTransactionCountByBlockNumber(int blockNumber)
        {
            var url = String.Format(ETHERSCAN_URL_GET_BLOCK_TRANSACTION_COUNT_BY_NUMBER, blockNumber.ToString("X"), ETHERSCAN_APIKEY);
            using (var client = new HttpClient(new RetryHandler(new HttpClientHandler())))
            {
                var response = await client.GetAsync(url);
                if (response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<GetBlockTransactionCountByNumberResponseDto>(content);

                        return Convert.ToInt32(data.result, 16);
                    }
                }
                throw new Exception($"Can't fetch transaction count for block {blockNumber}");
            }
        }

        public async Task<transaction> GetTransactionByBlockNumberAndIndex(block block, int indexOfTransaction)
        {
            var url = String.Format(ETHERSCAN_URL_GET_TRANSACTION_BY_BLOCKNUMBER_AND_INDEX, block.blockNumber.ToString("X"), "0x" + indexOfTransaction.ToString("X"), ETHERSCAN_APIKEY);

            using (var client = new HttpClient(new RetryHandler(new HttpClientHandler())))
            {
                var response = await client.GetAsync(url);
                if (response != null)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<GetTransactionByBlockNumberAndIndexResponseDto>(content);

                        return new transaction
                        {
                            blockID = block.blockID,
                            hash = data.result.hash,
                            fromAddress = data.result.from,
                            toAddress = data.result.to,
                            gas = (decimal)Convert.ToInt32(data.result.gas, 16),
                            gasPrice = (decimal)Convert.ToInt32(data.result.gas, 16),
                            transactionIndex = Convert.ToInt32(data.result.transactionIndex, 16),
                            value = Helper.ParseHexString(data.result.value)
                        };
                    }
                }
                throw new Exception($"Can't fetch transaction detail for block - {block.blockNumber} and transaction index {indexOfTransaction}");
            }
        }
    }
}
