var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
...
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();
