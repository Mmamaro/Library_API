using Library_API.Data;
using Library_API.Helpers;
using Library_API.Repositories;
using Library_API.Services;
using Library_API.Worker_Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

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
builder.Services.AddSingleton<IUser, UserRepo>();
builder.Services.AddSingleton<IAuth, AuthRepo>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddSingleton<TwoFaService>();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<BorrowingDueDateHelper>();

//Background Services
builder.Services.AddHostedService<DueDateReminderService>();

//Jwt configuration starts here
var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});
//Jwt configuration ends here

//Define Swagger generation options and add Bearer token authentication
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Library API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
});

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
