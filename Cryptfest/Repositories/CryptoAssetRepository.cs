using API.Data;
using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using Cryptfest.Data.Entities.Api;
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
            List<CryptoAsset> output = await _context.CryptoAssetInfo.ToListAsync();
            return output;
        }
        public ApiAccess GetApiAccess()
        {
            ApiAccess output = _context.ApiAccess.ToList().First() ;
            return output;
        }

        public async Task<CryptoAsset?> GetCryptoAssetBySymbolAsync(string symbol)
        {
            CryptoAsset? output = await _context.CryptoAssetInfo.FirstOrDefaultAsync(x => x.Symbol == symbol);
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
                .Include(x => x.User)
                    .ThenInclude(x => x.UserPersonalInfo)
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
    }
}
