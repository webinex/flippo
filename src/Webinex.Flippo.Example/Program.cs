using Webinex.Coded.AspNetCore;
using Webinex.Flippo;
using Webinex.Flippo.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add appsettings.Personal.json with Flippo:Path
builder.Configuration.AddJsonFile("appsettings.Personal.json", optional: false);

// Add services to the container.

builder.Services
    .AddCodedFailures()
    .AddControllers()
    .AddFlippoController();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFlippo(x => x
    .AddFileSystemBlob(builder.Configuration["Flippo:Path"])
    .UseSasToken(secret: "E701240A-E940-4B35-9ABE-D5D360B55A16", timeToLive: TimeSpan.FromMinutes(5)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();