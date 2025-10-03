using Tibber.Sdk;
using ZwembadControl.Connectors;
using ZwembadControl.Controllers;
using ZwembadControl.flows;
using ZwembadControl.Models;

namespace ZwembadControl.Rules
{
    public class BoilerFlow(ZwembadServiceActies zwembadServiceActies) : Flow(zwembadServiceActies)
    {
        public override async Task ExecuteAsync(PriceInfo priceInfo, decimal totalPrice, PriceLevel priceLevel, AirWellData airWellData, HyconData hyconData)
        {
            if (CurrentState.Instance.boilerMode == "auto")
            {
                if (priceLevel == PriceLevel.Expensive || priceLevel == PriceLevel.VeryExpensive)
                {
                    if (CurrentState.Instance.CurrentBoilerWaterTemp >= 50)
                    {
                        await zwembadServiceActies.CloseBoilerKlepAsync();
                    }
                    else
                    {
                        await zwembadServiceActies.OpenBoilerKlepAsync();
                    }
                }
                else
                {
                    if (CurrentState.Instance.CurrentBoilerWaterTemp >= 50)
                    {
                        await zwembadServiceActies.CloseBoilerKlepAsync();
                    }
                    else
                    {
                        await zwembadServiceActies.OpenBoilerKlepAsync();
                    }
                }
            }
        }
    }
}
