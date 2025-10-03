using Tibber.Sdk;
using ZwembadControl.Connectors;
using ZwembadControl.Controllers;
using ZwembadControl.flows;
using ZwembadControl.Models;

namespace ZwembadControl.Rules
{
    public class AirwellFlow(ZwembadServiceActies zwembadServiceActies) : Flow(zwembadServiceActies)
    {
        public override async Task ExecuteAsync(PriceInfo priceInfo, decimal totalPrice, PriceLevel priceLevel, AirWellData airWellData, HyconData hyconData)
        {
            if (CurrentState.Instance.airwellMode == "auto")
            {
                if (priceLevel == PriceLevel.Expensive || priceLevel == PriceLevel.VeryExpensive)
                {
                    if (CurrentState.Instance.CurrentBoilerWaterTemp < 40)
                    {
                        await zwembadServiceActies.StartAirwellWarmtePompasync();
                        await zwembadServiceActies.SetNormalTempAirwellWarmtePompasync();
                    }
                    else
                    {
                        await zwembadServiceActies.SetLowTempAirwellWarmtePompasync();
                    }
                }
                else if (priceLevel == PriceLevel.Normal)
                {
                    await zwembadServiceActies.StartAirwellWarmtePompasync();
                    await zwembadServiceActies.SetNormalTempAirwellWarmtePompasync();
                }
                else
                {
                    await zwembadServiceActies.StartAirwellWarmtePompasync();
                    await zwembadServiceActies.SetHighTempAirwellWarmtePompasync();
                }
            }
        }
    }
}
