using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using DevExpress.AspNetCore;
using FinalProject;
using FinalProject.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Reflection;

AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<RestClientConfiguration, RestClientConfiguration>();
builder.Services.AddHttpClient<MyService>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
    })
    .AddHttpMessageHandler<TokenHandler>();
builder.Services.AddScoped(typeof(IRestClient<>), typeof(RestClient<>));
builder.Services.AddScoped(typeof(IGenericApiClient<>), typeof(GenericApiClient<>));
builder.Services.AddScoped<TokenHandler>();

builder.Services.AddControllersWithViews().AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

//builder.Services.AddDevExpressControls();
builder.Services.AddMvc().AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddScoped<IClaimsTransformation, ClaimsTransformer>();

// Register Clients
var clientAssembly = Assembly.GetAssembly(typeof(AccountsApiClient));
builder.Services.AddClients(clientAssembly);

builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;
}
);

int Mins = 1800;
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.AccessDeniedPath = "/Users/login";
    options.LoginPath = "/Users/login";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(Mins);
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(Mins);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 1. Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",//MyAllowSpecificOrigins
        policy =>
        {
            policy.AllowAnyOrigin()//WithOrigins("https://example.com") // Add your allowed origins here
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

//...
builder.Services.AddDevExpressControls();
// ...
var app = builder.Build();
//...
app.UseDevExpressControls();
//...
//app.Run();


// 2. Use the CORS policy
app.UseCors("AllowAll");//MyAllowSpecificOrigins


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Show detailed errors in development
    app.UseDeveloperExceptionPage();
}
else
{
    // Show error page in production
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseNotyf();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Map default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
