using Serilog;

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
builder.Services
    .AddControllersWithViews()
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
app.UseRouting();

app.UseAuthorization();



// --------------------
// Routes
// --------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    ;

app.Run();
