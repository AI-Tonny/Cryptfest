using API.Data;
using API.Interfaces.Services.Crypto;
using Cryptfest.Interfaces.Repositories;
using Cryptfest.Interfaces.Services.InitialCall;
using Cryptfest.Interfaces.Services.User;
using Cryptfest.Interfaces.Validation;
using Cryptfest.Repositories;
using Cryptfest.ServiceImpementation;
using Cryptfest.ServiceImplementation;
using Cryptfest.Validation;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddScoped<ICryptoAssetRopository, CryptoAssetRepository>();


builder.Services.AddControllers();
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
    var initialCall = scope.ServiceProvider.GetRequiredService<IInitialCallService>();
    bool init = await initialCall.SaveAssetsInDbFromApi();                                         // ============> this may be an error
    if (init is false) { throw new InvalidOperationException(); } 

    // Save id db info for api (key - token)
    await initialCall.InitialApiAccess();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
