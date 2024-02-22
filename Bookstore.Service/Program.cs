using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Rhetos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRhetosHost((serviceProvider, rhetosHostBuilder) => rhetosHostBuilder
        .ConfigureRhetosAppDefaults()
        .UseBuilderLogProviderFromHost(serviceProvider)
        .ConfigureConfiguration(cfg => cfg.MapNetCoreConfiguration(builder.Configuration)))
    .AddAspNetCoreIdentityUser()
    .AddDashboard()
    .AddHostLogging()
    .AddRestApi(o =>
    {
        o.BaseRoute = "rest";
        o.GroupNameMapper = (conceptInfo, controller, oldName) => "rhetos";
    });

builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.ToString());
    c.SwaggerDoc("rhetos", new OpenApiInfo { Title = "Rhetos REST API", Version = "v1" });
    c.SwaggerDoc("Bookstore", new OpenApiInfo { Title = "Bookstore.Service", Version = "v1" });
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o => o.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    });

builder.Services.AddControllers()
    .AddNewtonsoftJson(o =>
    {
        o.UseMemberCasing();
        o.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;
        o.SerializerSettings.Converters.Add(new Rhetos.Host.AspNet.RestApi.Utilities.ByteArrayConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/rhetos/swagger.json", "Rhetos REST API");
        c.SwaggerEndpoint("/swagger/Bookstore/swagger.json", "Bookstore.Service");
    });
}

app.UseRhetosRestApi();

app.UseHttpsRedirection();

app.UseRouting();   

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints => { endpoints.MapRhetosDashboard(); });

app.MapControllers();

app.Run();
