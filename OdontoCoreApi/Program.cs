using Gbarber.Application;
using Gbarber.Application.Interfaces;
using GBarber.Core.Customers.Interfaces;
using GBarber.Core.Entities;
using GBarber.Core.Interfaces;
using GBarber.Core.Settings;
using GBarber.Infrastructure;
using GBarber.Infrastructure.Data;
using GBarber.Infrastructure.Identity.Services;
using GBarber.Infrastructure.Seeds;
using GBarber.Service.Services;
using GBarber.SQL.GenericRepository.Service;
using GBarber.SQL.Identity.Services;
using GBarber.WebApi.ConfigDependency.DB;
using GBarber.WebApi.ConfigDependency.Identity;
using GBarber.WebApi.ConfigDependency.JTW;
using GBarber.WebApi.ConfigDependency.Swagger;
using GBarber.WebApi.Filter;
using log4net.Config;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServicesDB(builder);

builder.Services.AddScoped<IAuthResponse, AuthResponseService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddApplicationServices();
builder.Services.Configure<MailSettings>(
    builder.Configuration.GetSection("MailSettings"));
//Jwt configuration ends here
builder.Services.AddApplicationServicesJwt(builder);

//Configure Log4net.
XmlConfigurator.Configure(new FileInfo("log4net.config"));
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof (FilterAuthorizationError));
});
builder.Services.AddApplicationServicesSwagger();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowDomains",
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader() 
                          .AllowAnyMethod();
                      });
});

builder.Services.AddApplicationServicesIdentity();

var app = builder.Build();
app.UseCors("AllowDomains");
using (IServiceScope? scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    var loggerFactory = service.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = service.GetRequiredService<AppDbContext>();
        var userManager = service.GetRequiredService<UserManager<AppUser>>();
        var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
        //await DefaultRoles.SeedRoles(roleManager);
        //await DefaultUsers.SeedUsers(userManager);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
