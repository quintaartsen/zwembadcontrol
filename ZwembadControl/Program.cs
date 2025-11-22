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
builder.Services.AddSingleton<RelayConnector, RelayConnector>();
builder.Services.AddSingleton<RelayController, RelayController>();
builder.Services.AddSingleton<ZwembadService, ZwembadService>();
builder.Services.AddSingleton<ZwembadServiceActies, ZwembadServiceActies>();
builder.Services.AddSingleton<IAirWellConnector, AirWellConnector>();
builder.Services.AddSingleton<IHyconConnector, HyconConnector>();
builder.Services.AddSingleton<ITibberConnector, TibberConnector>();
builder.Services.AddSingleton<IAcquaNetConnector, AcquaNetConnector>();
builder.Services.AddSingleton<AcquaNetConnector, AcquaNetConnector>();
builder.Services.AddSingleton<CurrentState, CurrentState>();

builder.Services.AddSingleton<AirwellFlow>();
builder.Services.AddSingleton<BoilerFlow>();
builder.Services.AddSingleton<KlimaatFlow>();
builder.Services.AddSingleton<LegionellaFlow>();
builder.Services.AddSingleton<ZwembadFlow>();

builder.Services.AddSingleton<IList<Flow>>(sp => new List<Flow>
{
    sp.GetRequiredService<AirwellFlow>(),
    sp.GetRequiredService<BoilerFlow>(),
    sp.GetRequiredService<KlimaatFlow>(),
    sp.GetRequiredService<LegionellaFlow>(),
    sp.GetRequiredService<ZwembadFlow>()
});

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





