using API.Data.Entities.UserEntities;
using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using Cryptfest.Data.Entities.Api;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    public DbSet<ApiAccess> ApiAccess { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserLogInfo> UserLogInfo { get; set; }
    public DbSet<UserPersonalInfo> UserPersonalInfo { get; set; }

    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<CryptoAssetInfo> CryptoAssetInfo { get; set; }
    public DbSet<CryptoTrade> CryptoTrades { get; set; }
    public DbSet<CryptoTransfer> CryptoTransfers { get; set; }
    private DbSet<CryptoAssetPrice> CryptoAssetPrices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Wallet>()
            .HasMany(x => x.Assets)
            .WithMany();



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
