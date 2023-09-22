using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(

    (provider, opt) =>
    {
        var env = provider.GetRequiredService<IHostEnvironment>();
        if (env.IsProduction())
        {
            var config = provider.GetRequiredService<IConfiguration>();

            opt.UseSqlServer(config.GetConnectionString("PlatformsConn"));
            Console.WriteLine(config.GetConnectionString("PlatformsConn"));
            Console.WriteLine("Using mssql db");

        }
        else
        {
            Console.WriteLine("Using in mem db");
            opt.UseInMemoryDatabase("InMem");

        }
    });
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<ICommandDataClient, HttpDataCommandClient>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
PrepDb.PrepPopulation(app, app.Environment.IsProduction());
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
