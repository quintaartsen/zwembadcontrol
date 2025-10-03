using Quartz.Impl;
using Quartz.Spi;
using Quartz;
using ZwembadControl.Connectors;
using ZwembadControl.Controllers;
using ZwembadControl.Jobs;
using ZwembadControl.Models;
using ZwembadControl.flows;
using ZwembadControl.Rules;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Set Injection mapping
builder.Services.AddSingleton<IRelayConnector, RelayConnector>();
builder.Services.AddSingleton<RelayController, RelayController>();
builder.Services.AddSingleton<ZwembadService, ZwembadService>();
builder.Services.AddSingleton<ZwembadServiceActies, ZwembadServiceActies>();
builder.Services.AddSingleton<IAirWellConnector, AirWellConnector>();
builder.Services.AddSingleton<IHyconConnector, HyconConnector>();
builder.Services.AddSingleton<ITibberConnector, TibberConnector>();
builder.Services.AddSingleton<IAcquaNetConnector, AcquaNetConnector>();
builder.Services.AddSingleton<CurrentState, CurrentState>();

builder.Services.AddSingleton<Flow, AirwellFlow>();
builder.Services.AddSingleton<Flow, BoilerFlow>();
builder.Services.AddSingleton<Flow, KlimaatFlow>();
builder.Services.AddSingleton<Flow, LegionellaFlow>();
builder.Services.AddSingleton<Flow, ZwembadFlow>();

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





