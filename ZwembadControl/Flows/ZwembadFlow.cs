using Tibber.Sdk;
using ZwembadControl.Connectors;
using ZwembadControl.Controllers;
using ZwembadControl.flows;
using ZwembadControl.Models;

namespace ZwembadControl.Rules;
public class ZwembadFlow(ZwembadServiceActies zwembadServiceActies) : Flow(zwembadServiceActies)
{
    private readonly double BufferRangeZwembad = 0.4;
    private readonly double Hysteresis = 0.2;
    private bool isHeatingAllowed = true;

    public override async Task ExecuteAsync(PriceInfo priceInfo, decimal totalPrice, PriceLevel priceLevel, AirWellData airWellData, HyconData hyconData)
    {
        double target = hyconData.TargetTemperature;
        switch (priceLevel)
        {
            case PriceLevel.Expensive:
            case PriceLevel.VeryExpensive:
                target -= BufferRangeZwembad; 
                break;
            case PriceLevel.Normal:
                break;
            default: // Cheap
                target += BufferRangeZwembad + (totalPrice <= 0 ? 1 : 0);
                break;
        }

        if (hyconData.CurrentTemperature >= target)
            isHeatingAllowed = false;
        else if (hyconData.CurrentTemperature <= target - Hysteresis)
            isHeatingAllowed = true;

        bool shouldHeat = isHeatingAllowed && hyconData.CurrentTemperature < target;

        var shouldOpenKlep = true;
        if (priceLevel == PriceLevel.Expensive || priceLevel == PriceLevel.VeryExpensive)
        {
            shouldOpenKlep = (hyconData.CurrentTemperature < hyconData.TargetTemperature - 2 * BufferRangeZwembad);
        }
        shouldOpenKlep = CurrentState.Instance.CurrentBoilerWaterTemp >= 48;

        if (shouldHeat)
            await StartHeatingAsync(shouldOpenKlep);
        else
            await StopHeatingAsync();
    }

    private async Task StartHeatingAsync(bool openKlep)
    {
        if (openKlep && CurrentState.Instance.ZwembadMode == "auto")
        {
            await zwembadServiceActies.OpenZwembadKlepAsync();
        }
        await zwembadServiceActies.StartZwembadWarmtePompAsync();
    }

    private async Task StopHeatingAsync()
    {
        if (CurrentState.Instance.ZwembadMode == "auto")
        {
            await zwembadServiceActies.CloseZwembadKlepAsync();
        }
        await zwembadServiceActies.StopZwembadWarmtePompAsync();
    }
}