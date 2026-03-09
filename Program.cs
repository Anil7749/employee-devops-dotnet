var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// ADD THIS → ECS needs /health to know container is alive
builder.Services.AddHealthChecks();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// ADD THIS → ECS ALB health check endpoint
app.MapHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// ADD THIS → needed for integration tests later
public partial class Program { }
