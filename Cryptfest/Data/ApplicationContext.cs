using API.Data.Entities.UserEntities;
using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserLogInfo> UserLogInfo => Set<UserLogInfo>();
    public DbSet<UserPersonalInfo> UserPersonalInfo => Set<UserPersonalInfo>();

    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<CryptoAssetInfo> CryptoAssetInfo => Set<CryptoAssetInfo>();
    public DbSet<CryptoTrade> CryptoTrades => Set<CryptoTrade>();
    public DbSet<CryptoTransfer> CryptoTransfers => Set<CryptoTransfer>();
    private DbSet<CryptoAssetPrice> CryptoAssetPrices => Set<CryptoAssetPrice>();

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
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
