using API.Data;
using API.Data.Entities.UserEntities;
using API.Data.Entities.Wallet;
using API.Data.Entities.WalletEntities;
using Cryptfest.Data.Entities.WalletEntities;
using Cryptfest.Interfaces.Repositories;
using Cryptfest.Interfaces.Services;
using Cryptfest.Interfaces.Validation;
using Cryptfest.Repositories;
using Cryptfest.Repositories;
using Cryptfest.ServiceImpementation;
using Cryptfest.ServiceImplementation;
using Cryptfest.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationContext>(context =>
{
    string sqlConnection = builder.Configuration.GetConnectionString("SqlConnectionString")!;
    context.UseSqlServer(sqlConnection);
});

builder.Services.AddAutoMapper(conf => { }, typeof(Program));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IUserValidation, UserValidation>();
builder.Services.AddScoped<ICryptoService, CryptoService>();
builder.Services.AddScoped<IInitialCallService, InitialCallService>();
builder.Services.AddScoped<ICryptoAssetRepository, CryptoAssetRepository>();
builder.Services.AddScoped<IApiService, ApiService>();

var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],

        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});


builder.Services.AddControllers()
    .AddJsonOptions(option =>
    {
        option.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Create db
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    //await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();

    // Take crypto assets from api and save in db
    if ( !(context.CryptoAssetInfo.Any() && context.ApiAccess.Any()) )
    {
        var initialCall = scope.ServiceProvider.GetRequiredService<IInitialCallService>();
        bool init = await initialCall.SaveAssetsInDbFromApi();                                         // ============> this may be an error
        if (init is false) { throw new InvalidOperationException(); }
        await initialCall.InitialApiAccess();
    }

    //Save id db info for api (key - token)

    if (!context.Wallets.Any())
    {

        Wallet wallet = new()
        {
            User = new User()
            {
                CreatedDate = new DateTime(2021, 12, 4, 10, 12, 44),
                UserLogInfo = new UserLogInfo()
                {
                    Login = "Vasya123",
                    HashPassword = "1234"
                },
                UserPersonalInfo = new UserPersonalInfo()
                {
                    Name = "Vasyl",
                    Surname = "Pupkin",
                    BirthDate = new DateTime(2004, 4, 16),
                },
            },
            Balances = new List<CryptoBalance>()
            {
                new CryptoBalance()
                {
                    Amount = 1.5m,
                    PurchasePrice = 100_000,
                    Asset = new()
                    {
                        Logo = "logo",
                        Name = "Bitcoin",
                        Symbol = "BTC",
                        MarketData = new()
                        {
                            CurrPrice = 110_000.123m,
                            PercentChange1h = 2.1m,
                            PercentChange24h = 2.1m,
                            PercentChange7d = 2.1m,
                            PercentChange30d = 2.1m,
                            PercentChange60d = 2.1m
                        }
                    },
                },
                 new CryptoBalance()
                {
                    Amount = 1m,
                    PurchasePrice = 4000m,
                    Asset = new()
                    {
                        Logo = "logo",
                        Name = "Ethereum",
                        Symbol = "ETH",
                        MarketData = new()
                        {
                            CurrPrice = 4012.123m,
                            PercentChange1h = 2.1m,
                            PercentChange24h = 2.1m,
                            PercentChange7d = 2.1m,
                            PercentChange30d = 2.1m,
                            PercentChange60d = 2.1m
                        }
                    },
                },
            },
            Statistic = new WalletStatistic()
            {
                TotalDeposit = 154_000m,
                TotalAssets = 0,
                Apy = 0,
                
            }
        };
        context.Add(wallet);
        context.SaveChanges();
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = ""; // Swagger буде відкриватись на "/"
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
