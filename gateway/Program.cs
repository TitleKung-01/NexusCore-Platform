using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// [SYSTEM]: ตั้งค่าให้โปรเจกต์ Gateway รันที่ Port 5000 เสมอ
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5000); 
});

// 1. เพิ่มระบบ YARP เข้าไปใน Application
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// 2. ตั้งค่าระบบป้องกันบอท/ถล่มยิง API (Rate Limiter)
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("StrictPolicy", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10); // ในเวลา 10 วินาที
        opt.PermitLimit = 5;                  // อนุญาตให้เรียกได้แค่ 5 ครั้งเท่านั้น
        opt.QueueLimit = 0;                   // เกินกว่านั้นตัดทิ้งทันที ไม่ให้ต่อคิว
    });
    
    // หากโดนบล็อก ให้ส่งกลับเป็น HTTP Status Code 429 (Too Many Requests)
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// 3. เพิ่มบริการ CORS ให้หน้าบ้านเชื่อมเข้ามาได้
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins(
                  "http://localhost:5173",
                  "http://localhost:3000",
                  "http://localhost:8080",
                  "http://localhost:8081")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// 4. เปิดใช้งาน Middleware ตามลำดับความปลอดภัย
app.UseRouting();
app.UseCors("AllowReact"); // เรียกใช้ CORS ก่อนเปิดประตู Proxy
app.UseRateLimiter(); // เช็กความถี่ก่อนเลย ถ้าผ่านค่อยไปต่อ

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "gateway" }));

// 5. สั่งให้เปิดประตู Gateway ส่ง Request ไปหลังบ้าน
app.MapReverseProxy();

app.Run();