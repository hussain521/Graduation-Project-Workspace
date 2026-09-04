using API;
using Infrastructure.Authorization;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext
/*builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));*/

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("AsmaFinalProjectInMemoryDB"));


builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

// Register Repository
var repositoryAssembly = Assembly.GetAssembly(typeof(AccountRepository));
builder.Services.AddRepositories(repositoryAssembly);

var mvcBuilder = builder.Services.AddMvc().AddJsonOptions(
    opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = null;
        opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    }
    );//.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
//builder.Services.AddAutoMapper(typeof(MappingProfile));

/*mvcBuilder.AddMvcOptions(o => o.Conventions.Add(new GenericControllerNameConvention()))
    .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

mvcBuilder.ConfigureApplicationPartManager(c =>
{
    c.FeatureProviders.Add(new GenericControllerFeatureProvider());
});*/

builder.Services.AddMvc().AddJsonOptions(
    opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = null;
        opts.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    }
    );

builder.Services.AddTransient<IClaimsTransformation, ClaimsTransformer>();

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllers();

// JWT
builder.Services.AddAuthentication(authOption =>
{
    authOption.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authOption.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtOptions =>
{
    var key = builder.Configuration.GetValue<string>("JwtConfig:Key");
    var keyBytes = Encoding.UTF8.GetBytes(key);
    jwtOptions.SaveToken = true;
    jwtOptions.TokenValidationParameters = new TokenValidationParameters()
    {
        /*IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateLifetime = true,
        ValidateAudience = false,
        ValidateIssuer = false,*/

        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        //ValidIssuer = "mytest.com",
        //ValidAudience = "mytest.com",
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };

    jwtOptions.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = async (context) =>
        {
            Result errorResult = new Result(false, new Error("Authentication Error"))
            {
                //ResponseCode = ResponseCode.UnAuthorized,
                //Data = null,
                IsSuccess = false,
                //Message = "Authentication Error",
                StatusCode = HttpStatusCode.Unauthorized,                
            };
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(errorResult);
        },
        OnChallenge = async (context) =>
        {
            Result errorResult = new Result(false, new Error("Authentication Error"))
            {
                //ResponseCode = ResponseCode.UnAuthorized,
                //Data = null,
                IsSuccess = false,
                //Message = "Authentication Error",
                StatusCode = HttpStatusCode.Unauthorized,
            };
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(errorResult);
        },
        OnForbidden = async (context) =>
        {
            Result errorResult = new Result(false, new Error("Authentication Error"))
            {
                //ResponseCode = ResponseCode.UnAuthorized,
                //Data = null,
                IsSuccess = false,
                //Message = "Authentication Error",
                StatusCode = HttpStatusCode.Unauthorized,
            };
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(errorResult);
        },
    };
});
//builder.Services.AddSingleton(typeof(IJwtTokenManger), typeof(JwtTokenManger));
builder.Services.AddScoped(typeof(IJwtTokenManger), typeof(JwtTokenManger));
// End Of JWT

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

try
{
    var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    SeedData.Initialize(scope.ServiceProvider, context);
}
catch (Exception ex)
{
    Console.WriteLine($"Database initialization bypassed or failed: {ex.Message}");
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app cors
app.UseRouting();
app.UseCors("corsapp");
//app.UseExceptionMiddleware();
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();