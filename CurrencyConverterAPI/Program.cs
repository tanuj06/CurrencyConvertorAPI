using AspNetCoreRateLimit;
using CurrencyConverterAPI.Middleware;
using CurrencyConverterAPI.Repos.IRepository;
using CurrencyConverterAPI.Repos.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

////JWT authentication
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters()
//        {
//            ValidateAudience = true,
//            ValidateIssuer = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//        };
//    });

// This allows only JSON and XML data request response
builder.Services.AddControllers(options => options.ReturnHttpNotAcceptable = false).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
        {
            new RateLimitRule
            {
                Endpoint = "*",
                Limit = 50, // Maximum number of requests per time span
                Period = "10m" // Time span (e.g., "1h" for 1 hour)
            }
        };
});

builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

builder.Services.AddHttpClient();
builder.Services.AddCors(p =>
{
    p.AddDefaultPolicy(options =>
    {
        options.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
    });
});
//builder.Services.AddScoped<MiddlewareService>();
builder.Services.AddScoped<ExceptionMiddleware>();
builder.Services.AddSingleton<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<ICurrencyMainService, CurrencyMainService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
//app.UseMiddleware<MiddlewareService>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseIpRateLimiting();

app.MapControllers();

app.Run();
