
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using messaging_app_API.Hubs;
using messaging_app_API.Services;
using messaging_app_API.UtilityServices;
using messaging_app_API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//  inmemory database for testing 
builder.Services.AddDbContext<Dbcontext>(option => option.UseInMemoryDatabase("UserDb"));
//builder.Services.AddDbContext<Dbcontext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ContactsApiConnectionString")));

builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using Beare scheme(\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey

    });
    option.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
        ValidateIssuer = false,
        ValidateAudience = false

    };
});

builder.Services.AddCors(option =>
{
    option.AddPolicy("mypolicy", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:4200/");
    });
});
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(builder =>
//    {
//        builder.WithOrigins("http://localhost:4200")
//               .AllowAnyHeader()
//               .AllowAnyMethod();
//    });
//});
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<ChatService>();

builder.Services.AddSignalR();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseCors("mypolicy");
app.UseAuthorization();


app.MapControllers();

app.MapHub<ChatHub>("/hubs/chat");
app.Run();
