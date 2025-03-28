using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ChatApp.API.Data;
using ChatApp.API.Extensions;
using ChatApp.API.Middleware;
using ChatApp.API.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocumentation();

// Add Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")),
        b => b.MigrationsAssembly("ChatApp.API")
    ));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder
                .WithOrigins("http://localhost:3000") // React开发服务器默认端口
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Add Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CaptchaService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));
builder.Services.AddScoped<PrivateMessageService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FriendshipService>();

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS
app.UseCors("AllowReactApp");

// Add Global Exception Handler
app.UseMiddleware<GlobalExceptionMiddleware>();

// Add Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// 添加静态文件服务
app.UseStaticFiles();

// 配置 CORS
app.UseCors(builder =>
{
    builder.WithOrigins("http://localhost:3000") // 前端开发服务器地址
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials();
});

app.MapControllers();

app.Run();
