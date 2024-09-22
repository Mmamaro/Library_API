using Library_API.Data;
using Library_API.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Registering Serilog
builder.Host.UseSerilog((context, configuration) =>
configuration.ReadFrom.Configuration(context.Configuration));

//Registering my services
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddSingleton<IGenre, GenreRepo>();
builder.Services.AddSingleton<IBook, BookRepo>();
builder.Services.AddSingleton<IBookCopy, BookCopyRepo>();
builder.Services.AddSingleton<ICustomer, CustomerRepo>();
builder.Services.AddSingleton<IBorrowing, BorrowingRepo>();
builder.Services.AddSingleton<IFine, FineRepo>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
