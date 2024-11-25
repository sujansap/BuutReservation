using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Persistence.Triggers;

using Rise.Services.Reservations;
using Rise.Services.TimeSlots;
using Rise.Shared.Reservations;
using Rise.Shared.TimeSlots;
using FluentValidation;
using FluentValidation.AspNetCore;
using Rise.Server.Middleware;
using Serilog.Events;
using Serilog;
using Rise.Shared.Notifications;
using Rise.Services.Notifications;
using Rise.Shared.Users;
using Rise.Services.Users;
using Rise.Shared.Notifications;
using Rise.Services.Notifications;

try
{
    Log.Information("Starting up Server");
    var builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .CreateLogger();

    builder.Services.AddSerilog();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath);
        options.EnableAnnotations();
    });

    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
        options.UseTriggers(options => options.AddTrigger<EntityBeforeSaveTrigger>());
    });

    builder.Services.AddScoped<ITimeSlotService, TimeSlotService>();
    builder.Services.AddScoped<IReservationService, ReservationService>();
    builder.Services.AddScoped<INotificationService, NotificationService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<INotificationService, NotificationService>();

    //validation using fluent validation
    builder.Services.AddValidatorsFromAssemblyContaining<CreateReservationDto.Validator>();
    builder.Services.AddFluentValidationAutoValidation();

    builder.Services.AddLocalization();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    if (!(app.Environment.IsProduction() || app.Environment.IsStaging()))
        app.UseHttpsRedirection();

    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();

    app.UseSerilogIngestion();

    app.UseMiddleware<ExceptionMiddleware>();

    app.UseRouting();

    app.MapControllers();
    app.MapFallbackToFile("index.html");

    if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
    {
        using var scope = app.Services.CreateScope();
        // Require a DbContext from the service provider and seed the database.
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        new Seeder(dbContext).Seed();
    }

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
public partial class Program { }
