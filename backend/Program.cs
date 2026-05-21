using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1. ลงทะเบียนระบบ Controllers (มีอยู่แล้วในเทมเพลต)
builder.Services.AddControllers();

// 2. ตั้งค่าระบบตรวจตั๋ว JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseRouting();

// 3. เปิดใช้งาน Authentication และ Authorization (ลำดับสำคัญมาก!)
app.UseAuthentication(); // ใครมา? (ตรวจตั๋ว)
app.UseAuthorization();  // ไปไหนได้บ้าง? (ตรวจสิทธิ์)

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "backend" }));

app.Run();