using EtherscanTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtherscanTest.Implementations
{
    public class DatabaseService : IDatabaseService
    {
        private IAppLogger appLogger;
        public DatabaseService(IAppLogger _appLogger)
        {
            appLogger = _appLogger;
        }
        public async Task StoreData(block block, List<transaction> transactions)
        {
            using (var context = new EtherscanTestEntities())
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        appLogger.LogInformation($"Store data of block number {block.blockNumber} and {transactions.Count} transactions");

                        var existedBlockID = await GetBlockIDByBlockNumber(block.blockNumber, context);
                        if (existedBlockID == 0)
                        {
                            context.blocks.Add(block);
                            await context.SaveChangesAsync();
                        }
                        else
                        {
                            block.blockID = existedBlockID;
                        }

                        foreach (var tx in transactions)
                        {
                            tx.blockID = block.blockID;
                        }

                        transactions = await FilterExistedTransactions(transactions, context);

                        if (transactions.Count > 0)
                        {
                            context.transactions.AddRange(transactions);
                            await context.SaveChangesAsync();
                        }

                        transaction.Commit();
                        appLogger.LogInformation($"Store info for block {block.blockNumber} & {transactions.Count} transactions successfully");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        appLogger.LogError(ex, $"Exception when store data of block {block.blockNumber} in database");
                    }
                }
            }
        }

        private async Task<List<transaction>> FilterExistedTransactions(List<transaction> transactions, EtherscanTestEntities context)
        {
            if (transactions.Count == 0)
                return transactions;

            var blockID = transactions.First().blockID;
            var hashes = transactions.Select(t => t.hash);

            var existedBlockTransactions = await context.transactions.Where(x => x.blockID == blockID && hashes.Contains(x.hash)).ToListAsync();

            if (existedBlockTransactions.Count == 0)
                return transactions;

            transactions = transactions.Except(existedBlockTransactions).ToList();
            return transactions;
        }

        private async Task<int> GetBlockIDByBlockNumber(int blockNumber, EtherscanTestEntities context)
        {
            var existedBlock = await context.blocks.FirstOrDefaultAsync(x => x.blockNumber == blockNumber);
            return existedBlock != null ? existedBlock.blockID: 0;
        }
    }
}