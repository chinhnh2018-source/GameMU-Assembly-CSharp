using GameMU.EventManager;
using GameMU.EventManager.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Services ────────────────────────────────────────────────────────────────
builder.Services.AddRazorPages();
builder.Services.AddSingleton<XmlEventService>();
builder.Services.AddSingleton<LinkResolutionService>();
builder.Services.AddSingleton<GoodsAuditService>();

// REST API: OpenAPI / Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title       = "GameMU.Manager API",
        Version     = "v1",
        Description = "REST API quản lý XML config files của GameMU Server. " +
                      "Đọc/ghi trực tiếp vào GameRes/Config/*.xml."
    });
});

// CORS cho Swagger UI và local tool scripts
builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p => p.WithOrigins("http://localhost:*", "http://127.0.0.1:*")
                                .AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// ── Middleware ───────────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseRouting();
app.UseCors();

// Swagger (chỉ trong Development hoặc luôn bật — tool nội bộ nên luôn bật)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GameMU.Manager API v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "GameMU.Manager API";
    c.DefaultModelsExpandDepth(-1); // ẩn schema section mặc định
});

// ── Routes ──────────────────────────────────────────────────────────────────
app.MapRazorPages();
app.MapApiEndpoints();   // /api/* endpoints

app.Run();
