var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Explicitly map static files for _framework in .NET 9 minimal hosting.
// This ensures the framework JS is served before routing/hub logic intercepts it.
app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/_framework"), appBuilder => {
    appBuilder.UseStaticFiles();
});

app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
