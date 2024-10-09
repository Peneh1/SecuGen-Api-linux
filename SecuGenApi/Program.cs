using beSmarter.BioTime.TransactionApi.Options;
using Dapper_ORM.Services;
using WebAPI1toN.Interfaces;
using WebAPI1toN.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Core SecuSearch library
builder.Services.AddSingleton<ISecuSearch3, SystemSecuSearch3>();
// Holds image data on search page across the top of the page + SessionID
builder.Services.AddSingleton<IImageContainer, ImageContainer>();
// Holds the sessionid, which is the key to load/saving WebSearch DB to disk.
builder.Services.AddSingleton<IDataPartitioning, DataPartitioning>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Dapper in scope    
builder.Services.AddScoped<IDapper, Dapperr>();

// Add your logging handler
builder.Logging.AddLog4Net();
// And the Options
builder.Services.Configure<LogOptions>(builder.Configuration.GetSection(LogOptions.Logging).GetSection(LogOptions.Options));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// Listen on all network interfaces
app.Urls.Add("http://0.0.0.0:5167"); // Added line

app.Run();
