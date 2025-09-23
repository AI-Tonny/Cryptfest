using API.Data;
using API.Data.Entities.Wallet;
using Cryptfest.Data.Entities.Api;
using Cryptfest.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cryptfest.Repositories
{
    public class CryptoAssetRepository : ICryptoAssetRopository
    {
        private readonly ApplicationContext _context;

        public CryptoAssetRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<CryptoAssetInfo>> GetCryptoAssetsAsync()
        {
            List<CryptoAssetInfo> output = await _context.CryptoAssetInfo.ToListAsync();
            return output;
        }
        public ApiAccess GetApiAccess()
        {
            ApiAccess output = _context.ApiAccess.ToList().First() ;
            return output;
        }

        public async Task<CryptoAssetInfo?> GetCryptoAssetBySymbolAsync(string symbol)
        {
            CryptoAssetInfo? output = await _context.CryptoAssetInfo.FirstOrDefaultAsync(x => x.Symbol == symbol);
            return output;
        }
    }
}
