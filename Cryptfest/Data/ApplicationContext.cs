using API.Data.Entities.UserEntities;
using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using Cryptfest.Data.Entities.ClientRequest;
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
    public DbSet<ClientRequest> ClientRequests { get; set; }


    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<CryptoAsset> CryptoAsset { get; set; }
    public DbSet<CryptoTransaction> CryptoExchanges { get; set; }
    public DbSet<CryptoBalance> CryptoBalances { get; set; }
    public DbSet<CryptoAssetMarketData> CryptoAssetMarketData { get; set; }
    private DbSet<WalletStatistic> WalletStatistics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Wallet>()
            .HasMany(x => x.Balances)
            .WithOne(x => x.Wallet)
            .HasForeignKey(x => x.WalletId);




        modelBuilder
            .Entity<CryptoTransaction>()
            .HasOne(x => x.FromAsset)
            .WithMany()
            .HasForeignKey(x => x.FromAssetId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        

        modelBuilder
            .Entity<CryptoTransaction>()
            .HasOne(x => x.ToAsset)
            .WithMany()
            .HasForeignKey(x => x.ToAssetId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}

