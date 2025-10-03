using API.Data.Entities.UserEntities;
using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using Cryptfest.Data.Entities.Api;
using Cryptfest.Data.Entities.WalletEntities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserLogInfo> UserLogInfo { get; set; }
    public DbSet<UserPersonalInfo> UserPersonalInfo { get; set; }

    public DbSet<ApiAccess> ApiAccess { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<CryptoAsset> CryptoAsset { get; set; }
    public DbSet<CryptoExchange> CryptoExchanges { get; set; }
    public DbSet<CryptoBalance> CryptoBalances { get; set; }
    public DbSet<CryptoAssetMarketData> CryptoAssetMarketData { get; set; }
    private DbSet<WalletStatistic> WalletStatistics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Wallet>()
            .HasMany(x => x.Balances)
            .WithOne(x => x.Wallet)
            .HasForeignKey(x => x.WalletId);




        //    modelBuilder
        //        .Entity<CryptoTransfer>()
        //        .HasOne(x => x.FromWallet)
        //        .WithMany()
        //        .HasForeignKey(x => x.FromWalletId)
        //        .OnDelete(DeleteBehavior.Restrict);

        //    modelBuilder
        //        .Entity<CryptoTransfer>()
        //        .HasOne(x => x.ToWallet)
        //        .WithMany()
        //        .HasForeignKey(x => x.ToWalletId)
        //        .OnDelete(DeleteBehavior.Restrict);
        //}
    }
}
