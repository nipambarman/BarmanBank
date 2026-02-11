using BarmanBank.Data;
using BarmanBank.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Razorpay.Api;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Configure Serilog
// --------------------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// --------------------
// Services
// --------------------

// Add DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Razorpay client registration must happen BEFORE TransactionService
builder.Services.AddScoped<RazorpayClient>(sp =>
{
    string key = builder.Configuration["Razorpay:Key"];
    string secret = builder.Configuration["Razorpay:Secret"];
    return new RazorpayClient(key, secret);
});

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAiInsightService, AiInsightService>();

// Add session support
builder.Services.AddSession();

// Add controllers with views + Razor runtime compilation
builder.Services.AddControllersWithViews()
       .AddRazorRuntimeCompilation();

var app = builder.Build();

// --------------------
// Middleware
// --------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Logs HTTP requests automatically
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession(); // <-- Session middleware

// --------------------
// Routes
// --------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

app.Run();
