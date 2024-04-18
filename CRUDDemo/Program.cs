using Services;
using ServiceContracts;
using Microsoft.EntityFrameworkCore;
using Entities;
using Serilog;
using Serilog.AspNetCore;
using CRUDDemo.Filter.ActionFilter;

var builder = WebApplication.CreateBuilder(args);

//logging
builder.Host.ConfigureLogging(loggingProvider =>
{
    loggingProvider.ClearProviders();
    loggingProvider.AddConsole();
    loggingProvider.AddDebug();
});


//Serilog
builder.Host.UseSerilog(HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration)//read configuration settings from built-in Iconfigurtion
      .ReadFrom.Services(services);//read out current app's services and make them available
});



builder.Services.AddControllersWithViews(options =>
{
    //options.Filters.Add<ResponseHeaderActionFilter>(5);

var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

    options.Filters.Add(new ResponseHeaderActionFilter(logger, "Key-from-Global", "Value-from-Global", 2));
});


//add services into IoC container
builder.Services.AddScoped<ICountryService, CountriesService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddDbContext<PersonsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddHttpLogging(options=> {

    options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
    
});
//Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PersonsDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False

var app = builder.Build();
app.UseSerilogRequestLogging();

//create application pipeline
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpLogging();

//app.Logger.LogDebug("debug-message");
//app.Logger.LogInformation("information-message");
//app.Logger.LogWarning("debug-message");
//app.Logger.LogCritical("critical-message");
//app.Logger.LogError("error-message");
//app.Logger.LogDebug("debug-message");

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
