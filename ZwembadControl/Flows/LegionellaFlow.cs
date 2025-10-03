using Tibber.Sdk;
using ZwembadControl.Connectors;
using ZwembadControl.Controllers;
using ZwembadControl.flows;
using ZwembadControl.Models;

namespace ZwembadControl.Rules
{
    public class LegionellaFlow(ZwembadServiceActies zwembadServiceActies, AcquaNetConnector acquaNetConnector, RelayConnector relayConnector) : Flow(zwembadServiceActies)
    {
        public override async Task ExecuteAsync(PriceInfo priceInfo, decimal totalPrice, PriceLevel priceLevel, AirWellData airWellData, HyconData hyconData)
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                var priceLow = decimal.MaxValue;
                var time = string.Empty;

                for (int i = 0; i < priceInfo.Today.Count - 3; i++)
                {
                    var price = priceInfo.Today.ElementAt(i);

                    var start = DateTime.Parse(price.StartsAt);
                    var calculatingPrice = price.Total + priceInfo.Today.ElementAt(i + 1).Total + priceInfo.Today.ElementAt(i + 2).Total;
                    if (calculatingPrice < priceLow)
                    {
                        priceLow = (decimal)calculatingPrice;
                        time = price.StartsAt;
                    }
                }

                if (priceInfo.Current.StartsAt == time)
                {
                    await zwembadServiceActies.StartLegionellasync();
                }
            }

            ///////////////////////////////////////Legionella mode////////////////////////////////////////////////////////////////////////
            if (CurrentState.Instance.LegionellaBoiler)
            {
                if (CurrentState.Instance.CurrentBoilerWaterTemp >= 64)
                {
                    CurrentState.Instance.LegionellaBoiler = false;
                    CurrentState.Instance.Spoelen = "uit";
                    await acquaNetConnector.StartSpoelenAsync();
                    relayConnector.OpenRelay(RelayConfig.LegionellaBoiler);
                }
            }
        }
    }
}
