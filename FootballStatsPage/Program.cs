var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// 1. Serve static files FIRST. This ensures _framework assets are accessible before routing/hub logic.
app.UseStaticFiles();

// 2. Skip HTTPS redirection in Development. Self-signed cert loops often strip request paths, causing 404s for framework files.
if (app.Environment.IsDevelopment())
{
    // Local dev runs cleanly on HTTP/HTTPS without redirect interference
}
else
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
