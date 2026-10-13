using FootballStatsFrontend.Components;

namespace FootballStatsFrontend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
                

            // Register HttpClient with a BaseAddress pointing to the backend API.
            // This resolves the "invalid request URI" error by providing a base URL for relative paths.
            var backendUrl = builder.Configuration["BackendUrl"] ?? "https://localhost:63068";
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(backendUrl) });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();

            // Map Razor components (Blazor UI) and automatically discover @page routes
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();
                

            app.Run();
        }
    }
}
