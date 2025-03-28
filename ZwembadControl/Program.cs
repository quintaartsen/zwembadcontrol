using Quartz.Impl;
using Quartz.Spi;
using Quartz;
using ZwembadControl.Connectors;
using ZwembadControl.Controllers;
using ZwembadControl.Jobs;
using ZwembadControl.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Set Injection mapping
builder.Services.AddSingleton<RelayConnector, RelayConnector>();
builder.Services.AddSingleton<RelayController, RelayController>();
builder.Services.AddSingleton<ZwembadService, ZwembadService>();
builder.Services.AddSingleton<AirWellConnector, AirWellConnector>();
builder.Services.AddSingleton<HyconConnector, HyconConnector>();
builder.Services.AddSingleton<TibberConnector, TibberConnector>();
builder.Services.AddSingleton<CurrentState, CurrentState>();

//Setup jobs

builder.Services.AddSingleton<IJobFactory, SingletonJobFactory>();
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
builder.Services.AddSingleton<MainJob>();
builder.Services.AddSingleton(new JobSchedule(
    typeof(MainJob),
    "0 * * ? * *")); // Run every 1 minute

builder.Services.AddHostedService<JobHostedService>();


// Voeg Razor Pages ondersteuning toe
builder.Services.AddRazorPages();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();

app.Run();





