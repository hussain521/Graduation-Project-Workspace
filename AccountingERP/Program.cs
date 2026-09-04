using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using AccountingERP.Data;
using AccountingERP.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Services to DI Container
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add Tenant Service
builder.Services.AddScoped<ITenantService, TenantService>();

// Add DbContext with SQLite (zero configuration, persistent database)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite("Data Source=AccountingErp.db");
});

// Add Business Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IUserService, UserService>();

// Authentication Configuration (Cookie Auth + JWT Bearer Support)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.Name = "AccountingERP.Auth";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

// Swagger API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "نظام السجلات والمحاسبة ERP API",
        Version = "v1",
        Description = "واجهات برمجة التطبيقات الذكية للوصول إلى الحسابات، القيود المحاسبية والتقارير المالية"
    });
});

var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.SeedAsync(context);
}

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Accounting ERP API V1");
    c.RoutePrefix = "swagger";
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();