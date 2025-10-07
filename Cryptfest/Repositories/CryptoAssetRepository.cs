using API.Data;
using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using Cryptfest.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cryptfest.Repositories
{
    public class CryptoAssetRepository : ICryptoAssetRepository
    {
        private readonly ApplicationContext _context;

        public CryptoAssetRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<CryptoAsset>> GetCryptoAssetsAsync()
        {
            List<CryptoAsset> output = await _context.CryptoAsset
                .Include(x => x.MarketData)
                .ToListAsync();
            return output;
        }

        public async Task<CryptoAsset?> GetCryptoAssetBySymbolAsync(string symbol)
        {
            CryptoAsset? output = await _context.CryptoAsset
                .Include(x => x.MarketData)
                .FirstOrDefaultAsync(x => x.Symbol == symbol);
            return output;
        }

        public async Task<Wallet?> GetWalletByIdAsync(int id)
        {
            Wallet? output = await _context.Wallets
                .Include(x => x.Balances)!
                    .ThenInclude(x => x.Asset)
                        .ThenInclude(x => x.MarketData)
                .Include(x => x.User)
                    .ThenInclude(x => x.UserLogInfo)
                .Include(x => x.Statistic)
                .FirstOrDefaultAsync(x => x.Id == id);

            return output;
        }


        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Update(object entity)
        {
            _context.Update(entity);
        }

        public async Task AddWalletAsync(Wallet wallet)
        {
            await _context.AddAsync(wallet);
        }

    }
}
