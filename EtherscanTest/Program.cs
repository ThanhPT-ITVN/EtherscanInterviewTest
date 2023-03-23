using EtherscanTest.Implementations;
using EtherscanTest.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace EtherscanTest
{
    class Program
    {
        private static int START_BLOCK = int.Parse(ConfigurationManager.AppSettings["START_BLOCK"]);
        private static int END_BLOCK = int.Parse(ConfigurationManager.AppSettings["END_BLOCK"]);
        static async Task Main(string[] args)
        {

            //Initialize Logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var serviceProvider = new ServiceCollection()
                .AddTransient<IEtherscanService, EtherscanService>()
                .AddTransient<IDatabaseService, DatabaseService>()
                .AddSingleton<IAppLogger, AppLogger>()
                .BuildServiceProvider();

            var etherscanService = serviceProvider.GetService<IEtherscanService>();
            var databaseService = serviceProvider.GetService<IDatabaseService>();
            var logger = serviceProvider.GetService<IAppLogger>();

            try
            {
                for (var i = START_BLOCK; i <= END_BLOCK; i++)
                {
                    logger.LogInformation($"Query Block and Transaction on block {i}");

                    var block = await etherscanService.GetBlockInfoByBlockNumber(i);
                    var transactions = new List<transaction>();

                    var blockTransactionCountByNumber = await etherscanService.GetBlockTransactionCountByBlockNumber(block.blockNumber);

                    if (blockTransactionCountByNumber != 0)
                    {
                        for (int txIndex = 0; txIndex < blockTransactionCountByNumber; txIndex++)
                        {
                            var blockTransaction = await etherscanService.GetTransactionByBlockNumberAndIndex(block, txIndex);
                            if (blockTransaction != null)
                            {
                                transactions.Add(blockTransaction);
                            }
                        }
                    }
                    await databaseService.StoreData(block, transactions);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Exception when query Block and Transaction");
            }
        }

    }
}
