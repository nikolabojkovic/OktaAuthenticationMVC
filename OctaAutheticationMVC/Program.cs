using OctaAutheticationMVC.Models;
using OctaAutheticationMVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<HipsoOktaSettings>(builder.Configuration.GetSection("HPSO_OKTA"));
builder.Services.Configure<NsoOktaSettings>(builder.Configuration.GetSection("NSO_OKTA"));
builder.Services.Configure<OktaSettings>(builder.Configuration.GetSection("OKTA"));
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration.GetSection("OKTA")
                                                          .GetValue<string>("ApiUrl"));
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
