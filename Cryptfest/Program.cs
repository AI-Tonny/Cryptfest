using API.Data;
using Cryptfest.Interfaces.Repositories;
using Cryptfest.Interfaces.Services;
using Cryptfest.Interfaces.Services.User;
using Cryptfest.Interfaces.Validation;
using Cryptfest.Repositories;
using Cryptfest.ServiceImpementation;
using Cryptfest.ServiceImplementation;
using Cryptfest.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://127.0.0.1:5500") 
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Configuration.AddJsonFile(
    path:"appsettings.apilinks.json",
    optional: false,
    reloadOnChange: true);

builder.Configuration.AddJsonFile(
    path: "appsettings.apitokens.json",
    optional: false,
    reloadOnChange: true);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

var app = builder.Build();

//додати перед авторизацією перед запитами!!!
app.UseCors("AllowFrontend");

using (var scope = app.Services.CreateScope())
{
    // Create db
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();

    // Take crypto assets from api and save in db
    if ( !(context.CryptoAsset.Any()) )
    {
        var initialCall = scope.ServiceProvider.GetRequiredService<IInitialCallService>();

        bool init = await initialCall.SaveAssetsInDbFromApi();                                      
        if (init is false) { throw new InvalidOperationException(); }
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

app.UseAuthorization();

app.MapControllers();

app.Run();
